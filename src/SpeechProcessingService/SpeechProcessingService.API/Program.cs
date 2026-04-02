using SpeechProcessingService.Infrastructure.Services;
using SpeechProcessingService.Application.Services.Interfaces;
using SpeechProcessingService.Application.Config;
using SpeechProcessingService.Application.Features.Audio.ProcessAudio;

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
builder.Services.AddScoped<IAsrService, GgmlWhisperService>();
builder.Services.AddScoped<IDateParser, DucklingClient>();

builder.Services.AddSingleton<IIntentClassificationService, IntentClassificationService>();
builder.Services.AddSingleton<IEntityExtractionService, EntityExtractionService>();
builder.Services.AddSingleton<IEntityNormalizer, EntityNormalizer>();
builder.Services.AddScoped<ISpeechProcessingService, SpeechProcessingService.Application.Services.SpeechProcessingService>();
builder.Services.AddSingleton<IGenAIService, GigaChatService>();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(
            new Newtonsoft.Json.Converters.StringEnumConverter());
    });

builder.Services.AddSwaggerGen();

var app = builder.Build();

var intentClassificationService = app.Services.GetRequiredService<IIntentClassificationService>();
await intentClassificationService.InitAsync();

var entityExtractionService = app.Services.GetRequiredService<IEntityExtractionService>();
await entityExtractionService.InitAsync();

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
