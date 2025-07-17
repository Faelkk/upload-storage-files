// Services/FileLocalService.cs
using FilesStorage.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace FilesStorage.Services;

public class FileLocalService : IFileLocalService
{
    private readonly IBus _bus;

    public FileLocalService(IBus bus)
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

            var message = new UploadLocalFileMessage
            {
                Filename = file.FileName,
                FileBytes = ms.ToArray(),
                ContentType = file.ContentType,
            };

            await _bus.Publish(message);
        }

        return (true, null);
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteFileAsync(string fileName)
    {
        if (
            string.IsNullOrEmpty(fileName)
            || fileName.Contains("..")
            || Path.GetFileName(fileName) != fileName
        )
            return (false, "Nome de arquivo inválido.");

        await _bus.Publish(new DeleteLocalFileMessage { Filename = fileName });

        return (true, null);
    }
}
