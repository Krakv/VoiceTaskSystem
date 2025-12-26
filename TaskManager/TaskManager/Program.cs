using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Features.TaskItem.CreateTask;
using TaskManager.Application.Features.TaskItem.ReadTaskList;
using TaskManager.Application.Services;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Config;
using TaskManager.Infrastructure.Repository;
using TaskManager.Application.Features.TaskItem.DeleteTask;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddHostedService<TelegramBotService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ISpeechProcessingClient, SpeechProcessingClient>();
builder.Services.AddScoped<IIntentDispatcher, IntentDispatcher>();
builder.Services.AddScoped<CreateTaskHandler>();
builder.Services.AddScoped<ReadTaskListHandler>();
builder.Services.AddScoped<DeleteTaskHandler>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<TelegramBotConfig>(builder.Configuration.GetSection("TelegramBot"));
builder.Services.Configure<SpeechProcessingConfig>(builder.Configuration.GetSection("SpeechProcessingConfig"));

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
