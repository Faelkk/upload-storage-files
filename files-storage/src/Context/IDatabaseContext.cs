using FilesStorage.Models;
using Microsoft.EntityFrameworkCore;

public interface IDatabaseContext
{
    public DbSet<FileRecord> FileRecords { get; set; }

    Task<int> SaveChangesAsync();
}
