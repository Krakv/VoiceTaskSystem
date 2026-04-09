using Microsoft.Extensions.Logging;
using TaskManager.Shared.EventHandlers;
using TaskManager.Shared.Events;

namespace TaskManager.TaskManagement.Application.Features.EventHandlers;

public class TaskCreatedPushHandler(ILogger<TaskCreatedPushHandler> logger) : BaseEventHandler<TaskCreatedEvent>(logger)
{
    private readonly ILogger<TaskCreatedPushHandler> _logger = logger;

    protected override Task Process(TaskCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[PUSH] Уведомление для пользователя {UserId}: Задача '{Title}' создана (ID {TaskId})",
            notification.UserId,
            notification.Title,
            notification.TaskId);
        return Task.CompletedTask;
    }
}
