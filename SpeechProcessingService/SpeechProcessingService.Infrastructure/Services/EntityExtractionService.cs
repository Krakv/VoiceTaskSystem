using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Tokenizers.DotNet;
using SpeechProcessingService.Application.Config;
using SpeechProcessingService.Application.Services.Interfaces;
using SpeechProcessingService.Infrastructure.Utils;

namespace SpeechProcessingService.Infrastructure.Services;

/// <summary>
/// Сервис для извелечения сущностей объекта "задача" из текстовой команды.
/// </summary>
/// <remarks>
/// Использует ONNX модель (RuRoBERTa) для предсказания и токенизатор Tokenizers.DotNet для преобразования текста в токены.
/// Поддерживает следующие метки сущностей:
/// TITLE, STATUS, PROJECT, PRIORITY, PARENT_TASK, DUE_DATE, DESCRIPTION.
/// </remarks>
public class EntityExtractionService: IEntityExtractionService
{
    private readonly ILogger<EntityExtractionService> _logger;
    private readonly NerOnnxModel _model;
    private readonly Dictionary<int, string> _id2label;
    private Tokenizer? _tokenizer;

    /// <summary>
    /// Создаёт новый экземпляр сервиса EntityExtractionService.
    /// </summary>
    /// <param name="model">Настройки модели ONNX через IOptions.</param>
    public EntityExtractionService(
        IOptions<NerOnnxModel> model,
        ILogger<EntityExtractionService> logger
        )
    {
        _model = model.Value;
        _logger = logger;

        // Словарь id - имя класса
        _id2label = new Dictionary<int, string>
        {
            {0,  "B-DESCRIPTION"},
            {1,  "B-DUE_DATE"},
            {2,  "B-PARENT_TASK"},
            {3,  "B-PRIORITY"},
            {4,  "B-PROJECT"},
            {5,  "B-STATUS"},
            {6,  "B-TITLE"},
            {7,  "I-DESCRIPTION"},
            {8,  "I-DUE_DATE"},
            {9,  "I-PARENT_TASK"},
            {10, "I-PRIORITY"},
            {11, "I-PROJECT"},
            {12, "I-STATUS"},
            {13, "I-TITLE"},
            {14, "O"}
        };
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
        if (!File.Exists(_model.TokenizerPath))
            throw new FileNotFoundException("Vocab file not found", _model.TokenizerPath);

        _tokenizer = new Tokenizer(_model.TokenizerPath);
    }

    /// <summary>
    /// Скачивает все необходимые файлы модели (ONNX, данные модели, токенизатор, словарь).
    /// </summary>
    private async Task InitModelAsync()
    {
        await FileDownloader.EnsureFileAsync(
            _model.ModelUrl,
            _model.ModelPath,
            _logger);

        await FileDownloader.EnsureFileAsync(
            _model.TokenizerUrl,
            _model.TokenizerPath,
            _logger);
    }

    /// <summary>
    /// Извлекает именованные сущности из текста команды.
    /// </summary>
    /// <returns>
    /// Словарь сущностей, например:
    /// { "TITLE": "сделать отчёт", "PRIORITY": "высокий", "DUE_DATE": "завтра" }
    /// </returns>
    public async Task<Dictionary<string, string>> ExtractEntitiesAsync(string commandText)
    {
        if (_tokenizer == null)
            throw new InvalidOperationException("Tokenizer not initialized");

        const int maxLen = 64;
        const uint padId = 1;

        // Encode возвращает uint[]
        var ids = _tokenizer.Encode(commandText);
        var originalLength = Math.Min(ids.Length, maxLen);

        // Padding до maxLen
        var paddedIds = new uint[maxLen];
        Array.Copy(ids, paddedIds, originalLength);
        for (int i = originalLength; i < maxLen; i++)
            paddedIds[i] = padId;

        // Декодируем каждый токен отдельно в строку
        var tokenList = new List<string>(originalLength);
        for (int i = 0; i < originalLength; i++)
            tokenList.Add(_tokenizer.Decode([paddedIds[i]]));

        var inputIds = paddedIds.Select(x => (long)x).ToArray();
        var attentionMask = paddedIds.Select((id, i) => i < originalLength ? 1L : 0L).ToArray();

        using var session = new InferenceSession(_model.ModelPath);

        var inputIdsTensor = new DenseTensor<long>(inputIds, [1, maxLen]);
        var attentionMaskTensor = new DenseTensor<long>(attentionMask, [1, maxLen]);

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids",      inputIdsTensor),
            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor)
        };

        using var results = session.Run(inputs);
        var output = results[0].AsEnumerable<float>().ToArray();
        int numLabels = _id2label.Count;

        // BIO-декодинг
        var entities = new Dictionary<string, string>();
        string? currentEntity = null;
        var currentIds = new List<uint>();

        for (int i = 0; i < originalLength; i++)
        {
            int offset = i * numLabels;
            var logits = output[offset..(offset + numLabels)];
            int predictedId = Array.IndexOf(logits, logits.Max());

            if (!_id2label.TryGetValue(predictedId, out var label))
                label = "O";

            if (label.StartsWith("B-"))
            {
                if (currentEntity != null)
                    entities[currentEntity] = _tokenizer.Decode(currentIds.ToArray());

                currentEntity = label[2..];
                currentIds = [(paddedIds[i])];
            }
            else if (label.StartsWith("I-") && currentEntity != null)
            {
                currentIds.Add(paddedIds[i]);
            }
            else
            {
                if (currentEntity != null)
                {
                    entities[currentEntity] = _tokenizer.Decode(currentIds.ToArray());
                    currentEntity = null;
                    currentIds.Clear();
                }
            }
        }

        if (currentEntity != null)
            entities[currentEntity] = _tokenizer.Decode(currentIds.ToArray());

        return entities;
    }
}
