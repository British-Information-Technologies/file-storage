using FileStorageService.www.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.www.Repositories;

public class FileRepository(ApplicationDbContext context)
{
	public static readonly int MAX_BLOCKS = 10_485_760;
	
	private readonly Queue<(string, Stream)> _creationQueue = new();
	private readonly Lock _countLock = new();
	
	public async Task<List<FileHandle>> GetAllFilesAsync()
	{
		return await context.FileHandles
			.Include(e => e.FileBlocks).ToListAsync();
	}
	
	public async Task<FileHandle> GetFileAsync(Guid id)
	{
		return await context.FileHandles
			.Include(e => e.FileBlocks).FirstAsync();
	}
	
	public Task<Guid?> TryNewFileAsync(string name, Stream reader)
	{
		var tcs = new TaskCompletionSource<Guid?>();

		lock (_creationQueue)
		{
			_creationQueue.Enqueue((name, reader));
		}

		Task.Run(async () =>
		{

			Stream stream;
			string name;
			
			lock (_creationQueue)
			{
				(name, stream) = _creationQueue.Dequeue();
			}
			
			var fileHandle = new FileHandle
			{
				Name = name
			};
			
			var handle = await CreateFileBlocks(stream, fileHandle);
			var currentFileCount = handle.FileBlocks.Count();
			handle.FileBlockCount = currentFileCount;

			lock (_countLock)
			{
				var count = context.FileBlocks.Count();

				if (count+currentFileCount > MAX_BLOCKS)
				{
					tcs.SetResult(null);
					return;
				}

				context.Add(fileHandle);
				context.SaveChangesAsync();
				
				tcs.SetResult(fileHandle.Id);
			}
		});

		return tcs.Task;
	}

	
	private async Task<FileHandle> CreateFileBlocks(Stream reader, FileHandle fileHandle)
	{
		var blockNumber = 0;
		var blocks = new List<FileBlock>();
		var buffer = new byte[1024];
		
		while (await reader.ReadAsync(buffer) > 0)
		{
			var block = new FileBlock
			{
				BlockNumber = blockNumber++,
				Data = buffer,
				FileHandle = fileHandle
			};
			fileHandle.FileBlocks.Add(block);
		}

		return fileHandle;
	}

	public async Task<FileHandle> GetFileHandle(Guid id)
	{
		return await context.FileHandles.FirstAsync(f => f.Id == id);
	}

	public async Task DeleteFileHandle(Guid id)
	{
		var handle = await context.FileHandles
			.Include(f => f.FileBlocks)
			.FirstAsync(f => f.Id == id);
		
		context.FileHandles.Remove(handle);

		await context.SaveChangesAsync();
	}
}