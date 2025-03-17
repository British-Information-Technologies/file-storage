using System.Diagnostics.CodeAnalysis;
using FileStorageService.www.Atttributes;
using FileStorageService.www.Data;
using FileStorageService.www.Models;
using FileStorageService.www.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.www.Controllers;

public class FilesController(
	ApplicationDbContext context,
	FileRepository fileRepository) : Controller
{
	private const long MaxFileSize = 7L * 1024L * 1024L * 1024L;
	
	// GET
	public async Task<IActionResult> Index(Guid? id)
	{
		if (id is not null)
		{
			return await ViewSingleFile((Guid)id);
		}

		var fileHandles = await fileRepository.GetAllFilesAsync();

		var handles = fileHandles.Select(e => new FileHandleModel
			{
				FileName = e.Name,
				BlockCount = e.FileBlockCount,
				Id = e.Id,
			})
			.ToList();

		var model = new FileHandleListModel
		{
			FileHandles = handles
		};

		return View(model);
	}

	private async Task<IActionResult> ViewSingleFile(Guid id)
	{
		var fileHandle = await fileRepository.GetFileAsync(id);

		var model = new FileHandleModel
		{
			FileName = fileHandle.Name,
			BlockCount = fileHandle.FileBlockCount,
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
	[DisableFormValueModelBinding]
	[RequestSizeLimit(MaxFileSize)]
	[RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
	public async Task<IActionResult> New(NewFileModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState.Values.SelectMany(v => v.Errors));
		}

		var name = model.Name;
		var stream = model.FileContents.OpenReadStream();
		
		var fileHandleId = await fileRepository.TryNewFileAsync(name, stream);

		if (fileHandleId != null)
			return RedirectToAction("Index", "Files", new { id = fileHandleId });

		return RedirectToAction("NotEnoughSpace");
	}

	public IActionResult NotEnoughSpace()
	{
		return View();
	}
	
}