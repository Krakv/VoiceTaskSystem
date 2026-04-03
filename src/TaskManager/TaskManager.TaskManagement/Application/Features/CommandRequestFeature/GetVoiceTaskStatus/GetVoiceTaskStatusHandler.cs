using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;

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
            throw new ValidationAppException("CANCELLED", "Запрос с указанным ID отменен");
        }

        if (command.Status == CommandRequestStatus.Pending || command.Status == CommandRequestStatus.Processing)
        {
            throw new ValidationAppException("PENDING", "Запрос еще обрабатывается");
        }

        if (command.Status == CommandRequestStatus.Accepted)
        {
            throw new ValidationAppException("ALREADY_CONFIRMED", "Команда уже принята");
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
                    payload = JsonSerializer.Deserialize<TaskCreateData>(command.Payload);
                    break;
                }
            case CommandIntent.TaskQuery:
                {
                    payload = JsonSerializer.Deserialize<TaskQueryData>(command.Payload);
                    break;
                }
            case CommandIntent.TaskUpdate:
                {
                    payload = JsonSerializer.Deserialize<TaskUpdateData>(command.Payload);
                    break;
                }
            case CommandIntent.TaskDelete:
                {
                    payload = JsonSerializer.Deserialize<TaskDeleteData>(command.Payload);
                    break;
                }
            default :
                {
                    payload = null;
                    break;
                }
        }

        return new GetVoiceTaskStatusResponse(command.Intent, payload);
    }
}
