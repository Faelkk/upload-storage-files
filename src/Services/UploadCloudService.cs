using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

public class UploadCloudService : IUploadCloudService
{
    private readonly Cloudinary _cloudinary;

    public UploadCloudService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true,
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return uploadResult.SecureUrl.ToString();
        }
        else
        {
            throw new Exception($"Falha no upload do arquivo: {uploadResult.Error?.Message}");
        }
    }

    public async Task<bool> DeleteFileAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

        return deleteResult.Result == "ok";
    }
}
