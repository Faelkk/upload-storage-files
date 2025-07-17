using FilesStorage.Messages;
using FilesStorage.Services;
using MassTransit;

public class UploadLocalFileConsumer : IConsumer<UploadLocalFileMessage>
{
    private readonly ISaveStorageService storageService;

    public UploadLocalFileConsumer(ISaveStorageService storageService)
    {
        this.storageService = storageService;
    }

    public async Task Consume(ConsumeContext<UploadLocalFileMessage> context)
    {
        var msg = context.Message;

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, msg.Filename);

        await File.WriteAllBytesAsync(filePath, msg.FileBytes);

        await storageService.SaveAsync(msg.Filename, msg.ContentType);

        Console.WriteLine($"[✔] Arquivo salvo localmente: {filePath}");
    }
}
