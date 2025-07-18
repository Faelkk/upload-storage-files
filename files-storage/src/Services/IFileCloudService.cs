namespace FilesStorage.Services;

public interface IFileCloudService
{
    Task<(bool IsSuccess, string? ErrorMessage)> UploadFilesAsync(List<IFormFile> files);
    Task<(bool IsSuccess, string? ErrorMessage)> DeleteFileAsync(string publicId, string fileName);
}
