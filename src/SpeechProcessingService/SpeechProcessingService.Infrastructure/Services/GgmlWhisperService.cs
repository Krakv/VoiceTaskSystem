using Microsoft.Extensions.Options;
using SpeechProcessingService.Application.Config;
using SpeechProcessingService.Application.DTOs;
using SpeechProcessingService.Application.Services.Interfaces;
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
            await ConvertToWavAsync(inputStream, tempPath);

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

    private static async Task ConvertToWavAsync(Stream input, string outputPath)
    {
        var samples = ReadWavSamples(input, out int sourceSampleRate, out int sourceChannels);

        if (sourceChannels > 1)
            samples = ConvertToMono(samples, sourceChannels);

        if (sourceSampleRate != 16000)
            samples = Resample(samples, sourceSampleRate, 16000);

        await WriteWavAsync(outputPath, samples, 16000);
    }

    private static float[] ReadWavSamples(Stream stream, out int sampleRate, out int channels)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        reader.ReadBytes(4);  // "RIFF"
        reader.ReadInt32();   // file size
        reader.ReadBytes(4);  // "WAVE"
        reader.ReadBytes(4);  // "fmt "
        reader.ReadInt32();   // chunk size
        reader.ReadInt16();   // audio format (1 = PCM)
        channels = reader.ReadInt16();
        sampleRate = reader.ReadInt32();
        reader.ReadInt32();   // byte rate
        reader.ReadInt16();   // block align
        int bitsPerSample = reader.ReadInt16();

        while (true)
        {
            var chunkId = new string(reader.ReadChars(4));
            int chunkSize = reader.ReadInt32();
            if (chunkId == "data") break;
            reader.ReadBytes(chunkSize);
        }

        var dataBytes = reader.ReadBytes(int.MaxValue / 2);
        var samples = new float[dataBytes.Length / (bitsPerSample / 8)];

        for (int i = 0; i < samples.Length; i++)
        {
            if (bitsPerSample == 16)
                samples[i] = BitConverter.ToInt16(dataBytes, i * 2) / 32768f;
            else if (bitsPerSample == 32)
                samples[i] = BitConverter.ToSingle(dataBytes, i * 4);
            else if (bitsPerSample == 8)
                samples[i] = (dataBytes[i] - 128) / 128f;
        }

        return samples;
    }

    private static float[] ConvertToMono(float[] samples, int channels)
    {
        var mono = new float[samples.Length / channels];
        for (int i = 0; i < mono.Length; i++)
        {
            float sum = 0;
            for (int c = 0; c < channels; c++)
                sum += samples[i * channels + c];
            mono[i] = sum / channels;
        }
        return mono;
    }

    private static float[] Resample(float[] samples, int fromRate, int toRate)
    {
        if (fromRate == toRate) return samples;

        double ratio = (double)toRate / fromRate;
        var resampled = new float[(int)(samples.Length * ratio)];

        for (int i = 0; i < resampled.Length; i++)
        {
            double srcIndex = i / ratio;
            int lower = (int)srcIndex;
            int upper = Math.Min(lower + 1, samples.Length - 1);
            double fraction = srcIndex - lower;
            resampled[i] = (float)(samples[lower] * (1 - fraction) + samples[upper] * fraction);
        }

        return resampled;
    }

    private static async Task WriteWavAsync(string outputPath, float[] samples, int sampleRate)
    {
        await using var writer = new BinaryWriter(File.Create(outputPath));
        int dataSize = samples.Length * 2;

        writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(36 + dataSize);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
        writer.Write(16);         // chunk size
        writer.Write((short)1);   // PCM
        writer.Write((short)1);   // mono
        writer.Write(sampleRate);
        writer.Write(sampleRate * 2); // byte rate
        writer.Write((short)2);   // block align
        writer.Write((short)16);  // bits per sample
        writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
        writer.Write(dataSize);

        foreach (var sample in samples)
            writer.Write((short)(Math.Clamp(sample, -1f, 1f) * 32767));
    }
}