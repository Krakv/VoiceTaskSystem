namespace SpeechProcessingService.Application.Config;

public class IntentOnnxModel
{
    public string ModelPath { get; set; } = string.Empty;
    public string ModelDataPath { get; set; } = string.Empty;
    public string TokenizerPath { get; set; } = string.Empty;
    public string VocabPath { get; set; } = string.Empty;

    public string ModelUrl { get; set; } = string.Empty;
    public string ModelDataUrl { get; set; } = string.Empty;
    public string TokenizerUrl { get; set; } = string.Empty;
    public string VocabUrl { get; set; } = string.Empty;
}
