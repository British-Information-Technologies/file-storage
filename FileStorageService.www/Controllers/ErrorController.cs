using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.www.Controllers;

public class ErrorController : Controller
{
	// GET
	public IActionResult Status(int id)
	{
		return View(id);
	}
}