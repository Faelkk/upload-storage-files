// Services/SaveStorageService.cs
using FilesStorage.Models;
using FilesStorage.Repositories;

namespace FilesStorage.Services;

public class SaveStorageService : ISaveStorageService
{
    private readonly IFileRepository _repository;

    public SaveStorageService(IFileRepository repository)
    {
        _repository = repository;
    }

    public async Task SaveAsync(string fileName, string contentType)
    {
        var fileRecord = new FileRecord { FileName = fileName, ContentType = contentType };

        await _repository.AddAsync(fileRecord);
    }

    public async Task<FileRecord?> GetByNameAsync(string fileName)
    {
        return await _repository.GetByNameAsync(fileName);
    }

    public async Task<List<FileRecord>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task DeleteAsync(string fileName)
    {
        var file = await _repository.GetByNameAsync(fileName);
        if (file is not null)
            await _repository.DeleteAsync(file);
    }
}
