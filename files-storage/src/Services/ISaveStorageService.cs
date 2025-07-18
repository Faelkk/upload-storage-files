using FilesStorage.Models;

namespace FilesStorage.Services;

public interface ISaveStorageService
{
    Task SaveAsync(string fileName, string contentType);
    Task<FileRecord?> GetByNameAsync(string fileName);
    Task<List<FileRecord>> GetAllAsync();
    Task DeleteAsync(string fileName);
}
