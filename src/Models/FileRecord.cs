namespace FilesStorage.Models;

public class FileRecord
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
