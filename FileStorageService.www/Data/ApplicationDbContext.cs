using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.www.Data;

public class ApplicationDbContext(
	DbContextOptions<ApplicationDbContext> options)
	: DbContext(options)
{
	 public DbSet<FileBlock> FileBlocks { get; set; }
	 public DbSet<FileHandle> FileHandles { get; set; }
	 
}