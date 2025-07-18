// Services/IFileLocalService.cs
using Microsoft.AspNetCore.Http;

namespace FilesStorage.Services;

public interface IFileLocalService
{
    Task<(bool IsSuccess, string? ErrorMessage)> UploadFilesAsync(List<IFormFile> files);
    Task<(bool IsSuccess, string? ErrorMessage)> DeleteFileAsync(string fileName);
}
