// Repositories/IFileRepository.cs
using FilesStorage.Models;

namespace FilesStorage.Repositories;

public interface IFileRepository
{
    Task AddAsync(FileRecord file);
    Task<FileRecord?> GetByNameAsync(string fileName);
    Task<List<FileRecord>> GetAllAsync();
    Task DeleteAsync(FileRecord file);
}
