namespace FileStorageService.www.Models;

public class FileHandleDeleteModel
{
	public required Guid Id { get; init; }
	public required string FileName { get; init; }
	public string ConfirmFileName { get; init; } = "";
	public int BlockCount { get; set; }
}