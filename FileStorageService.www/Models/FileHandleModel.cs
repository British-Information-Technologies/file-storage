namespace FileStorageService.www.Models;

public class FileHandleModel
{
	public required string FileName { get; init; }
	
	public required int BlockCount { get; init; }
	public required Guid Id { get; init; }
}