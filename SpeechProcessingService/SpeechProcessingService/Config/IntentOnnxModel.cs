namespace SpeechProcessingService.Config;

public class IntentOnnxModel
{
    public string Path { get; set; } = "intent_model.onnx";
    public string TokenizerPath { get; set; } = "intent_tokenizer.json";
    public string VocabPath { get; set; } = "intent_vocab.txt";

}
