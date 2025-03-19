using System.Security.Cryptography.X509Certificates;
using FileStorageService.www.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.www.Repositories;

public class FileRepository(ApplicationDbContext context)
{
	public static readonly int MAX_BLOCKS = 10_485_760;
	
	private readonly Queue<(string, Stream, TaskCompletionSource<Guid?>)> _creationQueue = new();
	private readonly Lock _countLock = new();
	
	public async Task<List<FileHandle>> GetAllFilesAsync()
	{
		return await context.FileHandles.ToListAsync();
	}
	
	public async Task<FileHandle> GetFileAsync(Guid id)
	{
		return await (from handle in context.FileHandles
				where handle.Id == id
				select handle).FirstAsync();
	}
	
	public Task<string> GetFileName(Guid id)
	{
		return (from handle in context.FileHandles
			where handle.Id == id
			select handle.Name).FirstAsync();
	}
	
	public Stream EnumerateFileContentsAsync(Guid id)
	{

		var memoryStream = new MemoryStream();
		var writer = new BinaryWriter(memoryStream);
		
		var query = (from block in context.FileBlocks
			where block.FileHandle.Id == id
			orderby block.BlockNumber 
			select block.Data);

		foreach (var bytes in query.AsEnumerable())
		{
			writer.Write(bytes);
			writer.Flush();
		}
		
		memoryStream.Position = 0;

		return memoryStream;
	}
	
	public Task<Guid?> TryNewFileAsync(string name, Stream reader)
	{
		var tcs = new TaskCompletionSource<Guid?>();

		lock (_creationQueue)
		{
			_creationQueue.Enqueue((name, reader, tcs));
		}

		Task.Run(async () => await StartProcessFileUpload());

		return tcs.Task;
	}

	private async Task StartProcessFileUpload()
	{
		Stream stream;
		string name;
		TaskCompletionSource<Guid?> tcs;
			
		lock (_creationQueue)
		{
			(name, stream, tcs) = _creationQueue.Dequeue();
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
	}
	
	private async Task<FileHandle> CreateFileBlocks(Stream reader, FileHandle fileHandle)
	{
		var blockNumber = 0;
		var buffer = new byte[1024];
		
		while (await reader.ReadAsync(buffer) > 0)
		{
			var block = new FileBlock
			{
				BlockNumber = blockNumber++,
				Data = buffer.ToArray(),
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