namespace TaskManager.Application.Features.CommandRequestItem.CreateVoiceTask;

public class InputFile
{
    public string FileName { get; }
    public string ContentType { get; }
    public long Length { get; }
    public Stream Content { get; }

    public InputFile(
        string fileName,
        string contentType,
        long length,
        Stream content)
    {
        FileName = fileName;
        ContentType = contentType;
        Length = length;
        Content = content;
    }
}
