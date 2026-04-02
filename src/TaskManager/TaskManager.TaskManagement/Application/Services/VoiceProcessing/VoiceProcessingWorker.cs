using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManager.TaskManagement.Application.Services.Interfaces;

namespace TaskManager.TaskManagement.Application.Services.VoiceProcessing;

public class VoiceProcessingWorker(IBackgroundTaskQueue queue, IServiceProvider sp, ILogger<VoiceProcessingWorker> logger) : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue = queue;
    private readonly IServiceProvider _sp = sp;
    private readonly ILogger<VoiceProcessingWorker> _logger = logger;
    private readonly SemaphoreSlim _semaphore = new(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Started to handle voice requests");
            var job = await _queue.DequeueAsync(stoppingToken);
            await _semaphore.WaitAsync(stoppingToken);

            _logger.LogInformation("Running voice request");
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<VoiceProcessingHandler>();
                    await handler.Handle(job);
                }
                finally
                {
                    _semaphore.Release();
                }
            }, stoppingToken);
        }
    }
}
