using System.Diagnostics.CodeAnalysis;
using FileStorageService.www.Data;
using FileStorageService.www.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.www.Controllers;

public class FilesController(ApplicationDbContext context) : Controller
{
	// GET
	public async Task<IActionResult> Index(Guid? id)
	{
		if (id != null)
		{
			return await ViewSingleFile(id);
		}
		
		var handles = await context.FileHandles
			.Include(e => e.FileBlocks)
			.Select(e => new FileHandleModel
			{
				FileName = e.Name,
				BlockCount = e.FileBlocks.Count,
				Id = e.Id,
			})
			.ToListAsync();

		var model = new FileHandleListModel
		{
			FileHandles = handles
		};
		
		return View(model);
	}

	private async Task<IActionResult> ViewSingleFile([DisallowNull] Guid? id)
	{
		var fileHandle = await context.FileHandles
			.Include(e => e.FileBlocks)
			.FirstAsync(e => e.Id ==id);

		var model = new FileHandleModel
		{
			FileName = fileHandle.Name,
			BlockCount = fileHandle.FileBlocks.Count,
			Id = fileHandle.Id,
		};

		return View("SingleFile", model);
	}

	public IActionResult New()
	{
		return View(new NewFileModel
		{
			Name = "",
			FileContents = null!
		});
	}

	[HttpPost]
	public async Task<IActionResult> New(NewFileModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState.Values.SelectMany(v => v.Errors));
		}

		var handle = new FileHandle
		{
			Name = model.Name
		};
		context.Add(handle);
		
		var readStream = model.FileContents.OpenReadStream();
		
		await CreateFileBlocks(handle, readStream);
		
		await context.SaveChangesAsync();
		
		return Redirect($"/Files/{handle.Id}");
	}

	private async Task CreateFileBlocks(FileHandle fileHandle, Stream reader)
	{

		var blockNumber = 0;
		
		var buffer = new byte[1024];

		while (await reader.ReadAsync(buffer) > 0)
		{
			var block = new FileBlock
			{
				BlockNumber = blockNumber++,
				Data = buffer,
				FileHandle = fileHandle
			};

			// context.Add(block);

			fileHandle.FileBlocks.Add(block);
		}
	}
}