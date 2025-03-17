using System.ComponentModel.DataAnnotations;

namespace FileStorageService.www.Data;

public class FileHandle(string name)
{
	public Guid Id { get; set; }
	
	[MaxLength(64)]
	public string Name { get; set; } = name;

	public ICollection<FileBlock> Blocks { get; } = [];
}