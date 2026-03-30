using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using SpeechProcessingService.Application.Services.Interfaces;
using SpeechProcessingService.Config;

namespace SpeechProcessingService.Application.Services;

/// <summary>
/// Сервис для классификации намерений пользователя по текстовой команде.
/// </summary>
/// <remarks>
/// Использует ONNX модель для предсказания и токенизатор BERT для преобразования текста в токены.
/// Поддерживает стандартные метки намерений:
/// TASK_CREATE, TASK_UPDATE, TASK_DELETE, TASK_QUERY, UNKNOWN, AMBIGUOUS.
/// </remarks>
public class IntentClassificationService : IIntentClassificationService
{
    private readonly IntentOnnxModel _model;
    private readonly Dictionary<int, string> _id2label;
    private readonly BertTokenizer _tokenizer;

    /// <summary>
    /// Создаёт новый экземпляр сервиса IntentClassificationService.
    /// </summary>
    /// <param name="model">Настройки модели ONNX через IOptions.</param>
    /// <param name="tokenizer">Инициализированный токенизатор BERT (может быть синглтоном).</param>
    public IntentClassificationService(IOptions<IntentOnnxModel> model, BertTokenizer tokenizer)
    {
        _model = model.Value;
        _tokenizer = tokenizer;

        // Словарь id - имя класса
        _id2label = new Dictionary<int, string>
        {
            {0, "TASK_CREATE"},
            {1, "TASK_UPDATE"},
            {2, "TASK_DELETE"},
            {3, "TASK_QUERY"},
            {4, "UNKNOWN"},
            {5, "AMBIGUOUS"}
        };
    }

    /// <summary>
    /// Классифицирует намерение пользователя по текстовой команде.
    /// </summary>
    /// <param name="commandText">Текст команды или запроса пользователя.</param>
    /// <returns>
    /// Строковое название предсказанного класса намерения.
    /// Например: "TASK_CREATE", "TASK_UPDATE", "TASK_DELETE", "TASK_QUERY", "UNKNOWN" или "AMBIGUOUS".
    /// </returns>
    public async Task<string> ClassifyIntentAsync(string commandText)
    {
        // Токенизация текста
        var ids = _tokenizer.EncodeToIds(commandText);

        const int maxLen = 64;
        var padded = ids.Take(maxLen).ToList();

        // Padding
        if (padded.Count < maxLen)
        {
            padded.AddRange(Enumerable.Repeat(_tokenizer.PaddingTokenId, maxLen - padded.Count));
        }

        // Attention mask
        var attentionMask = padded
            .Select(id => id == _tokenizer.PaddingTokenId ? 0L : 1L)
            .ToArray();

        // Преобразуем в long тензор
        var inputIds = padded.Select(x => (long)x).ToArray();

        // Прогон через ONNX
        using var session = new InferenceSession(_model.Path);

        var inputIdsTensor = new DenseTensor<long>(inputIds, new int[] { 1, maxLen });
        var attentionMaskTensor = new DenseTensor<long>(attentionMask, new int[] { 1, maxLen });

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor)
        };

        using var results = session.Run(inputs);

        var output = results[0].AsEnumerable<float>().ToArray();

        // Получаем индекс класса с максимальной вероятностью
        int predictedClass = Array.IndexOf(output, output.Max());

        // Возвращаем имя класса
        return _id2label.TryGetValue(predictedClass, out var label)
            ? label
            : "UNKNOWN";
    }
}