
namespace SpeechProcessingService.Application.DTOs;

public class AudioFile(string fileName, string contentType, byte[] content)
{
    public string FileName { get; } = fileName;
    public string ContentType { get; } = contentType;
    public byte[] Content { get; } = content;
}
