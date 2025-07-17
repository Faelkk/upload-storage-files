using FilesStorage.Messages;
using FilesStorage.Services;
using MassTransit;

public class UploadCloudFileConsumer : IConsumer<UploadFileMessage>
{
    private readonly IUploadCloudService uploadCloudService;
    private readonly ISaveStorageService storageService;

    public UploadCloudFileConsumer(
        IUploadCloudService uploadCloudService,
        ISaveStorageService storageService
    )
    {
        this.uploadCloudService = uploadCloudService;
        this.storageService = storageService;
    }

    public async Task Consume(ConsumeContext<UploadFileMessage> context)
    {
        var msg = context.Message;

        using var stream = new MemoryStream(msg.FileBytes);

        var url = await uploadCloudService.UploadFileAsync(msg.Filename, stream);

        await storageService.SaveAsync(msg.Filename, msg.ContentType);

        Console.WriteLine($"[✔] Arquivo salvo: {msg.Filename}");
    }
}
