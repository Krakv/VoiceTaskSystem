using Microsoft.Extensions.Options;
using Microsoft.ML.Tokenizers;
using SpeechProcessingService.Infrastructure.Services;
using SpeechProcessingService.Application.Services.Interfaces;
using Whisper.net.Ggml;
using SpeechProcessingService.Application.Config;
using SpeechProcessingService.Application.Features.Audio.ProcessAudio;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

builder.Services.Configure<GigaChatCredentials>(builder.Configuration.GetSection("GigaChatCredentials"));
builder.Services.Configure<GgmlModel>(builder.Configuration.GetSection("GgmlModel"));
builder.Services.Configure<IntentOnnxModel>(builder.Configuration.GetSection("IntentOnnxModel"));

builder.Configuration.AddEnvironmentVariables();

#endregion Configuration

#region MediatR

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(ProcessAudioCommandHandler).Assembly
    );
});

#endregion MediatR

builder.Services.AddHttpClient();
builder.Services.AddScoped<IAsrService, GgmlWhisperService>();

builder.Services.AddSingleton<IIntentClassificationService, IntentClassificationService>();
builder.Services.AddScoped<IEntityExtractionService, EntityExtractionService>();
builder.Services.AddScoped<ISpeechProcessingService, SpeechProcessingService.Application.Services.SpeechProcessingService>();
builder.Services.AddSingleton<IGenAIService, GigaChatService>();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

var intentClassificationService = app.Services.GetRequiredService<IIntentClassificationService>();
await intentClassificationService.InitAsync();

using (var scope = app.Services.CreateScope())
{
    var asrService = scope.ServiceProvider.GetRequiredService<IAsrService>() as GgmlWhisperService;
    await asrService!.EnsureModelDownloadedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
