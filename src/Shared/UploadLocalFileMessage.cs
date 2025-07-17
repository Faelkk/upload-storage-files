namespace FilesStorage.Messages;

public class UploadLocalFileMessage
{
    public string Filename { get; set; } = default!;
    public byte[] FileBytes { get; set; } = default!;
    public string ContentType { get; set; } = default!;
}
