using FileStorageService.www.Data;
using FileStorageService.www.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.www.Test.Repositories;

public class FileRepositoryTests
{
	private readonly ApplicationDbContext _context;

	public FileRepositoryTests()
	{
		var connection = new SqliteConnection("Filename=:memory:");
		connection.Open();
		
		var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseSqlite(connection)
			.Options;

		_context = new ApplicationDbContext(contextOptions);
		
		_context.Database.EnsureDeleted();
		_context.Database.EnsureCreated();
	}

	private FileRepository repository => new FileRepository(_context);
	
	
	[Theory]
	[InlineData("one", 1)]
	[InlineData("two", 2)]
	[InlineData("four", 4)]
	[InlineData("eight", 8)]
	public async Task TestCanCreateNewFile(string name, int blockCount)
	{
		var data = new byte[blockCount * 1024];

		var stream = new MemoryStream(data);
		
		var id = await repository.TryNewFileAsync(name, stream);

		var handle = await _context.FileHandles.Include(f => f.FileBlocks).FirstAsync();
		
		Assert.Equal(name, handle.Name);
		Assert.Equal(blockCount, handle.FileBlocks.Count);
		Assert.Equal(blockCount, handle.FileBlockCount);
	}
	
	[Fact]
	public async Task TestFetchingFiles()
	{
		var name = "FileName";
		
		var handle = new FileHandle
		{
			Name = name,
			FileBlockCount = 1
		};
		_context.Add(handle);
			
		var block = new FileBlock
		{
			BlockNumber = 0,
			Data =  new byte[1024],
			FileHandle = handle
		};
		_context.Add(block);

		await _context.SaveChangesAsync();

		var fetchedHandle = await repository.GetFileAsync(handle.Id);
		
		Assert.Equal(name, fetchedHandle.Name);
		Assert.Equal(1, fetchedHandle.FileBlockCount);
		Assert.Single(fetchedHandle.FileBlocks);
	}
}