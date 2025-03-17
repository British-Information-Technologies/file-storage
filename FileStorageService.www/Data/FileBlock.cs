using System.ComponentModel.DataAnnotations;

namespace FileStorageService.www.Data;

/**
 * A block of data, used as prt of a full [FileHandle]
 */
public class FileBlock
{
	public Guid Id { get; init; }
	
	public required int BlockNumber { get; init; }
	
	[DataType("BLOB")]
	[MaxLength(1024)]
	public required byte[] Data { get; init; }
	
	public required FileHandle FileHandle { get; init; }
	
}
