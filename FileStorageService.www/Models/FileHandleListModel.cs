using FileStorageService.www.Data;
using FileStorageService.www.Repositories;

namespace FileStorageService.www.Models;

public class FileHandleListModel
{
	public required List<FileHandleModel> FileHandles { get; init; }
	public int CurrentBlockCount => FileHandles.Select(s => s.BlockCount).Sum();
	public int AllocatedBlockCount => FileRepository.MAX_BLOCKS;
	public int BlockUsagePercentage => (int)Math.Ceiling(((float)CurrentBlockCount / AllocatedBlockCount)*100);
}