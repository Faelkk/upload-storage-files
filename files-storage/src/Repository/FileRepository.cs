using FilesStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FilesStorage.Repositories;

public class FileRepository : IFileRepository
{
    private readonly IDatabaseContext _context;

    public FileRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(FileRecord file)
    {
        _context.FileRecords.Add(file);
        await _context.SaveChangesAsync();
    }

    public async Task<FileRecord?> GetByNameAsync(string fileName)
    {
        return await _context.FileRecords.FirstOrDefaultAsync(f => f.FileName == fileName);
    }

    public async Task<List<FileRecord>> GetAllAsync()
    {
        return await _context.FileRecords.ToListAsync();
    }

    public async Task DeleteAsync(FileRecord file)
    {
        _context.FileRecords.Remove(file);
        await _context.SaveChangesAsync();
    }
}
