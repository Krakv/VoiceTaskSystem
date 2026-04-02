using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using SpeechProcessingService.Application.Services.Interfaces;
using SpeechProcessingService.Application.Config;
using Microsoft.Extensions.Logging;
using SpeechProcessingService.Infrastructure.Utils;
using SpeechProcessingService.Application.DTOs.Responses;

namespace SpeechProcessingService.Infrastructure.Services;

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
    private readonly ILogger<IntentClassificationService> _logger;
    private BertTokenizer? _tokenizer;

    /// <summary>
    /// Создаёт новый экземпляр сервиса IntentClassificationService.
    /// </summary>
    /// <param name="model">Настройки модели ONNX через IOptions.</param>
    public IntentClassificationService(
        IOptions<IntentOnnxModel> model,
        ILogger<IntentClassificationService> logger)
    {
        _model = model.Value;
        _logger = logger;
    }

    /// <summary>
    /// Инициализирует сервис: скачивает модель и создает токенизатор.
    /// </summary>
    public async Task InitAsync()
    {
        await InitModelAsync();
        await InitTokenizerAsync();
    }

    /// <summary>
    /// Инициализирует токенизатор BERT на основе скачанного vocab-файла.
    /// </summary>
    /// <exception cref="FileNotFoundException">
    /// Бросается, если vocab-файл не найден по указанному пути.
    /// </exception>
    private async Task InitTokenizerAsync()
    {
        if (!File.Exists(_model.VocabPath))
            throw new FileNotFoundException("Vocab file not found", _model.VocabPath);

        _tokenizer = await BertTokenizer.CreateAsync(_model.VocabPath);
    }

    /// <summary>
    /// Скачивает все необходимые файлы модели (ONNX, данные модели, токенизатор, словарь).
    /// </summary>
    private async Task InitModelAsync()
    {
        Directory.CreateDirectory("Models");

        await FileDownloader.EnsureFileAsync(
            _model.ModelUrl,
            _model.ModelPath,
            _logger);

        await FileDownloader.EnsureFileAsync(
            _model.ModelDataUrl,
            _model.ModelDataPath,
            _logger);

        await FileDownloader.EnsureFileAsync(
            _model.TokenizerUrl,
            _model.TokenizerPath,
            _logger);

        await FileDownloader.EnsureFileAsync(
            _model.VocabUrl,
            _model.VocabPath,
            _logger);
    }

/// <summary>
/// Классифицирует намерение пользователя по текстовой команде.
/// </summary>
/// <param name="commandText">Текст команды или запроса пользователя.</param>
/// <returns>
/// Строковое название предсказанного класса намерения.
/// Например: "TASK_CREATE", "TASK_UPDATE", "TASK_DELETE", "TASK_QUERY", "UNKNOWN" или "AMBIGUOUS".
/// </returns>
public async Task<CommandIntent> ClassifyIntentAsync(string commandText)
    {
        if (_tokenizer == null) throw new TypeInitializationException(typeof(BertTokenizer).ToString(), new Exception("Tokenizer not initialized"));

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
        using var session = new InferenceSession(_model.ModelPath);

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
        int predictedClass = Array.IndexOf(output, output.Max()) + 1;

        return (CommandIntent)predictedClass;
    }
}