using FileStorageService.www.Data;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.www.Repositories;

public class FileRepository(ApplicationDbContext context)
{
	private const int MAX_BLOCKS = 1024;
	
	private Queue<(string, Stream)> creationQueue = new();
	private Lock countLock = new();
	
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

		lock (creationQueue)
		{
			creationQueue.Enqueue((name, reader));
		}

		Task.Run(async () =>
		{

			Stream stream;
			string name;
			
			lock (creationQueue)
			{
				(name, stream) = creationQueue.Dequeue();
			}
			
			var fileHandle = new FileHandle
			{
				Name = name
			};
			
			var handle = await CreateFileBlocks(stream, fileHandle);
			var currentFileCount = handle.FileBlocks.Count();

			lock (countLock)
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
}