using System.ComponentModel.DataAnnotations;

namespace FileStorageService.www.Models;

public class NewFileModel
{
	
	[MinLength(4)]
	[Required]
	public string Name { get; init; }
	
	[Required]
	public IFormFile FileContents { get; init; }
}