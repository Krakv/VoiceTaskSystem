namespace SpeechProcessingService.Application.Config;

public class NerOnnxModel
{
    public string ModelPath { get; set; } = string.Empty;
    public string MergesPath { get; set; } = string.Empty;
    public string TokenizerPath { get; set; } = string.Empty;
    public string VocabPath { get; set; } = string.Empty;

    public string ModelUrl { get; set; } = string.Empty;
    public string MergesUrl { get; set; } = string.Empty;
    public string TokenizerUrl { get; set; } = string.Empty;
    public string VocabUrl { get; set; } = string.Empty;
}