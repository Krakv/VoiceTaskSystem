using Microsoft.Extensions.Options;
using SpeechProcessingService.Application.Config;
using SpeechProcessingService.Application.DTOs;
using SpeechProcessingService.Application.Services.Interfaces;
using System.Diagnostics;
using Whisper.net;
using Whisper.net.Ggml;

namespace SpeechProcessingService.Infrastructure.Services;

public class GgmlWhisperService(IOptions<GgmlModel> model, IHttpClientFactory httpClientFactory) : IAsrService
{
    private readonly WhisperFactory _factory = WhisperFactory.FromPath(model.Value.Path);
    private readonly WhisperGgmlDownloader _downloader = new(httpClientFactory.CreateClient());
    private readonly string _modelPath = model.Value.Path;
    private WhisperProcessor? _processor;

    public async Task InitAsync()
    {
        await EnsureModelDownloadedAsync();
        _processor = _factory
                .CreateBuilder()
                .WithLanguage("ru")
                .Build();
    }

    public async Task EnsureModelDownloadedAsync()
    {
        if (File.Exists(_modelPath)) return;

        var directory = Path.GetDirectoryName(_modelPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using var modelStream = await _downloader.GetGgmlModelAsync(GgmlType.Small);
        using var fileStream = File.Create(_modelPath);
        await modelStream.CopyToAsync(fileStream);
    }

    public async Task<string> RecognizeSpeechAsync(AudioFile audioFile)
    {
        if (_processor == null) throw new TypeInitializationException(typeof(WhisperProcessor).ToString(), new Exception("Whisper processor not initialized"));

        var tempPath = Path.GetRandomFileName() + ".wav";

        try
        {
            using var inputStream = new MemoryStream(audioFile.Content);
            await ConvertWebmToWavAsync(inputStream, tempPath);

            var segments = new List<string>();

            await using var wavStream = File.OpenRead(tempPath);
            await foreach (var segment in _processor.ProcessAsync(wavStream))
            {
                segments.Add(segment.Text);
            }

            return string.Join(" ", segments).Trim();
        }
        finally
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    private static async Task ConvertWebmToWavAsync(Stream webmStream, string outputPath)
    {
        // Сохраняем входной поток во временный файл
        var tempWebm = Path.GetTempFileName() + ".webm";
        await using (var fileStream = File.Create(tempWebm))
        {
            await webmStream.CopyToAsync(fileStream);
        }

        // Запускаем ffmpeg
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i \"{tempWebm}\" -ar 16000 -ac 1 \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string stderr = await process.StandardError.ReadToEndAsync();
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception($"FFmpeg conversion failed: {stderr}");

        File.Delete(tempWebm);
    }
}