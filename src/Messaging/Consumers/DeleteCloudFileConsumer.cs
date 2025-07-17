using FilesStorage.Messages;
using FilesStorage.Services;
using MassTransit;

public class DeleteCloudFileConsumer : IConsumer<DeleteFileMessageCloud>
{
    private readonly IUploadCloudService uploadCloudService;
    private readonly ISaveStorageService storageService;

    public DeleteCloudFileConsumer(
        IUploadCloudService uploadCloudService,
        ISaveStorageService storageService
    )
    {
        this.uploadCloudService = uploadCloudService;
        this.storageService = storageService;
    }

    public async Task Consume(ConsumeContext<DeleteFileMessageCloud> context)
    {
        var publicId = context.Message.PublicId;

        await uploadCloudService.DeleteFileAsync(publicId);

        await storageService.DeleteAsync(context.Message.FileName);

        Console.WriteLine($"[🗑] Arquivo deletado  via service: {context.Message.FileName}");
    }
}
