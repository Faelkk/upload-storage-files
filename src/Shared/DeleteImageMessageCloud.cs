namespace FilesStorage.Messages;

public class DeleteFileMessageCloud
{
    public required string PublicId { get; set; }

    public required string FileName { get; set; }
}
