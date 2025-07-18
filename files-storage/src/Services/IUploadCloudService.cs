public interface IUploadCloudService
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream);
    Task<bool> DeleteFileAsync(string publicId);
}
