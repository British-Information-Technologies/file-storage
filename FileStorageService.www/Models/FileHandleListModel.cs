using FileStorageService.www.Data;

namespace FileStorageService.www.Models;

public class FileHandleListModel
{
	public required List<FileHandleModel> FileHandles { get; init; }
	public int CurrentBlockCount => FileHandles.Select(s => s.BlockCount).Sum();
	public int AllocatedBlockCount => 1024;
	public int BlockUsagePercentage => (int)Math.Ceiling(((float)CurrentBlockCount / AllocatedBlockCount)*100);
}