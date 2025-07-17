using FilesStorage.Messages;
using FilesStorage.Services;
using MassTransit;

public class DeleteLocalFileConsumer : IConsumer<DeleteLocalFileMessage>
{
    private readonly ISaveStorageService storageService;

    public DeleteLocalFileConsumer(ISaveStorageService storageService)
    {
        this.storageService = storageService;
    }

    public async Task Consume(ConsumeContext<DeleteLocalFileMessage> context)
    {
        var msg = context.Message;
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        var filePath = Path.Combine(uploadsFolder, msg.Filename);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Console.WriteLine($"[🗑] Arquivo deletado localmente: {filePath}");
        }
        else
        {
            Console.WriteLine($"[⚠] Arquivo não encontrado: {filePath}");
        }

        await storageService.DeleteAsync(msg.Filename);

        Console.WriteLine($"[🗑] Arquivo deletado localmente via service: {msg.Filename}");
    }
}
