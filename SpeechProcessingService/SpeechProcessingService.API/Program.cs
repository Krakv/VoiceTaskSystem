using Microsoft.Extensions.Options;
using Microsoft.ML.Tokenizers;
using SpeechProcessingService.API.Config;
using SpeechProcessingService.Application.Services;
using SpeechProcessingService.Application.Services.Interfaces;
using Whisper.net.Ggml;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

builder.Services.Configure<GigaChatCredentials>(builder.Configuration.GetSection("GigaChatCredentials"));
builder.Services.Configure<GgmlModel>(builder.Configuration.GetSection("GgmlModel"));
builder.Services.Configure<IntentOnnxModel>(builder.Configuration.GetSection("IntentOnnxModel"));

builder.Configuration.AddEnvironmentVariables();

#endregion Configuration

#region MediatR

builder.Services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(Program).Assembly));

#endregion MediatR

builder.Services.AddHttpClient();
builder.Services.AddScoped<IAsrService, GgmlWhisperService>();

builder.Services.AddScoped<IIntentClassificationService, IntentClassificationService>();
builder.Services.AddScoped<IEntityExtractionService, EntityExtractionService>();
builder.Services.AddScoped<ISpeechProcessingService, SpeechProcessingService.Application.Services.SpeechProcessingService>();
builder.Services.AddSingleton<IGenAIService, GigaChatService>();

#region ML-tools

builder.Services.AddSingleton(provider =>
{
    var model = provider.GetRequiredService<IOptions<IntentOnnxModel>>().Value;
    return BertTokenizer.Create(vocabFilePath: model.VocabPath);
});

#endregion ML-tools

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

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
