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

        // Caminho relativo ao projeto real
        var uploadsFolder = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "files-storage",
            "UploadedFiles"
        );
        uploadsFolder = Path.GetFullPath(uploadsFolder);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, msg.Filename);

        Console.WriteLine(
            "[CONSUMER] UploadLocalFileConsumer recebeu mensagem e vai salvar o arquivo!"
        );

        await File.WriteAllBytesAsync(filePath, msg.FileBytes);

        await storageService.SaveAsync(msg.Filename, msg.ContentType);

        Console.WriteLine($"[✔] Arquivo salvo localmente: {filePath}");
    }
}
