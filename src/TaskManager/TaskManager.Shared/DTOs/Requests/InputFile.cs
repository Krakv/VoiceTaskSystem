namespace TaskManager.Shared.DTOs.Requests;

public class InputFile
{
    public string FileName { get; }
    public string ContentType { get; }
    public long Length { get; }
    public byte[] Content { get; }

    public InputFile(
        string fileName,
        string contentType,
        long length,
        byte[] content)
    {
        FileName = fileName;
        ContentType = contentType;
        Length = length;
        Content = content;
    }
}
