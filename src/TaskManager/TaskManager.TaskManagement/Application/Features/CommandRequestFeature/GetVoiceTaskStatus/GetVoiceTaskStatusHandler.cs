using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Models;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public sealed class GetVoiceTaskStatusHandler(AppDbContext dbContext, ILogger<GetVoiceTaskStatusHandler> logger) : IRequestHandler<GetVoiceTaskStatusQuery, GetVoiceTaskStatusResponse>
{
    private readonly AppDbContext _dbcontext = dbContext;
    private readonly ILogger<GetVoiceTaskStatusHandler> _logger = logger;
    public async Task<GetVoiceTaskStatusResponse> Handle(GetVoiceTaskStatusQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Started to check status: {CommandRequestId} command", request.commandRequestId);
        var command = await _dbcontext.CommandRequestItem.FindAsync(Guid.Parse(request.commandRequestId), cancellationToken);

        if (command == null)
        {
            throw new ValidationAppException("NOT_FOUND", "Запрос с указанным ID не найден");
        }

        if (command.Status == CommandRequestStatus.Cancelled)
        {
            throw new ValidationAppException("NOT_FOUND", "Запрос с указанным ID отменен");
        }

        if (command.Status == CommandRequestStatus.Pending || command.Status == CommandRequestStatus.Processing)
        {
            throw new ValidationAppException("PENDING", "Запрос еще обрабатывается");
        }

        if (command.Status == CommandRequestStatus.Failed || command.Intent == null || command.Payload == null)
        {
            throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Не удалось обработать команду");
        }

        IVoiceTaskPayload? payload = null;

        switch (command.Intent)
        {
            case CommandIntent.TaskCreate:
                {
                    TaskItemModel? tempPayload = JsonSerializer.Deserialize<TaskItemModel>(command.Payload);

                    if (tempPayload == null)
                    {
                        throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Не удалось получить команду");
                    }

                    payload = ToTaskCreateData(tempPayload, null, $"Текст распознан {command.CreatedAt}");
                    break;
                }
            case CommandIntent.TaskQuery:
                {
                    // TODO
                    break;
                }
            default :
                {
                    payload = new TaskDeleteData();
                    break;
                }
        }

        return new GetVoiceTaskStatusResponse(command.Intent, payload);
    }

    public static TaskCreateData ToTaskCreateData(
        TaskItemModel model,
        Guid? parentTaskId,
        string message = "",
        bool confirmationRequired = true)
    {
        return new TaskCreateData(
            Title: model.Title ?? string.Empty,
            Description: model.Description,
            Status: model.Status?.ToString() ?? TaskItemStatus.New.ToString(),
            Priority: model.Priority?.ToString() ?? TaskItemPriority.Low.ToString(),
            DueDate: model.DueDate,
            Message: message,
            ParentTaskId: parentTaskId,
            ConfirmationRequired: confirmationRequired
        );
    }
}
