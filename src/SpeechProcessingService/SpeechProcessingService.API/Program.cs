using SpeechProcessingService.Application.Config;
using SpeechProcessingService.Application.Features.Audio.ProcessAudio;
using SpeechProcessingService.Application.Services.Interfaces;
using SpeechProcessingService.Infrastructure.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

builder.Services.Configure<GigaChatCredentials>(builder.Configuration.GetSection("GigaChatCredentials"));
builder.Services.Configure<GgmlModel>(builder.Configuration.GetSection("GgmlModel"));
builder.Services.Configure<IntentOnnxModel>(builder.Configuration.GetSection("IntentOnnxModel"));
builder.Services.Configure<NerOnnxModel>(builder.Configuration.GetSection("NerOnnxModel"));

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
builder.Services.AddScoped<IDateParser, DucklingClient>();

builder.Services.AddSingleton<IAsrService, GgmlWhisperService>();
builder.Services.AddSingleton<IIntentClassificationService, IntentClassificationService>();
builder.Services.AddSingleton<IEntityExtractionService, EntityExtractionService>();
builder.Services.AddSingleton<IEntityNormalizer, EntityNormalizer>();
builder.Services.AddSingleton<ISpeechProcessingService, SpeechProcessingService.Application.Services.SpeechProcessingService>();
builder.Services.AddSingleton<IGenAIService, GigaChatService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

var asrService = app.Services.GetRequiredService<IAsrService>();
await asrService.InitAsync();

var intentClassificationService = app.Services.GetRequiredService<IIntentClassificationService>();
await intentClassificationService.InitAsync();

var entityExtractionService = app.Services.GetRequiredService<IEntityExtractionService>();
await entityExtractionService.InitAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
