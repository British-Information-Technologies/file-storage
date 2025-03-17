using System.ComponentModel.DataAnnotations;

namespace FileStorageService.www.Data;

public class FileHandle
{
	public Guid Id { get; init; }
	
	[MaxLength(64)]
	public required string Name { get; set; }

	public ICollection<FileBlock> FileBlocks { get; } = new List<FileBlock>();

	public int FileBlockCount { get; set; } = 0;
}