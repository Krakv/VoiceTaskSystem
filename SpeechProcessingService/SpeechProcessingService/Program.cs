using SpeechProcessingService.Application.Services;
using SpeechProcessingService.Application.Services.Interfaces;
using SpeechProcessingService.Config;
using Whisper.net.Ggml;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GigaChatCredentials>(builder.Configuration.GetSection("GigaChatCredentials"));

builder.Services.Configure<GgmlModel>(builder.Configuration.GetSection("GgmlModel"));

builder.Configuration.AddEnvironmentVariables();

#region MediatR

builder.Services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(Program).Assembly));

#endregion MediatR

builder.Services.AddHttpClient();
builder.Services.AddScoped<IAsrService, GgmlWhisperService>();

builder.Services.AddScoped<IIntentClassificationService, IntentClassificationService>();
builder.Services.AddScoped<IEntityExtractionService, EntityExtractionService>();
builder.Services.AddScoped<ISpeechProcessingService, SpeechProcessingService.Application.Services.SpeechProcessingService>();
builder.Services.AddSingleton<IGenAIService, GigaChatService>();

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
