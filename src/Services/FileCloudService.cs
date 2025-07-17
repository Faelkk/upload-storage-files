using FilesStorage.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace FilesStorage.Services;

public class FileCloudService : IFileCloudService
{
    private readonly IBus _bus;

    public FileCloudService(IBus bus)
    {
        _bus = bus;
    }

    private readonly string[] permittedExtensions = [".jpg", ".jpeg", ".png", ".svg"];
    private readonly string[] permittedContentTypes = ["image/jpeg", "image/png", "image/svg+xml"];

    public async Task<(bool IsSuccess, string? ErrorMessage)> UploadFilesAsync(
        List<IFormFile> files
    )
    {
        if (files == null || files.Count == 0)
            return (false, "Nenhum arquivo recebido.");

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                return (false, $"Extensão não permitida: {file.FileName}");

            if (!permittedContentTypes.Contains(file.ContentType))
                return (false, $"Tipo MIME não permitido: {file.FileName}");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var message = new UploadFileMessage
            {
                Filename = file.FileName,
                FileBytes = ms.ToArray(),
                ContentType = file.ContentType,
            };

            await _bus.Publish(message);
        }

        return (true, null);
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteFileAsync(
        string publicId,
        string fileName
    )
    {
        if (string.IsNullOrEmpty(publicId))
            return (false, "PublicId não fornecido.");

        await _bus.Publish(new DeleteFileMessageCloud { PublicId = publicId, FileName = fileName });

        return (true, null);
    }
}
