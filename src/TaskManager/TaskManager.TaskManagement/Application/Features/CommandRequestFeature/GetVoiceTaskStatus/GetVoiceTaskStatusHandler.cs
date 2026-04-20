using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Utils;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public sealed class GetVoiceTaskStatusHandler(AppDbContext dbContext, ILogger<GetVoiceTaskStatusHandler> logger) : IRequestHandler<GetVoiceTaskStatusQuery, GetVoiceTaskStatusResponse>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<GetVoiceTaskStatusHandler> _logger = logger;
    public async Task<GetVoiceTaskStatusResponse> Handle(GetVoiceTaskStatusQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Started to check status: {CommandRequestId} command", request.CommandRequestId);
        var command = await _dbContext.CommandRequestItem
            .Where(r => r.CommandRequestId == request.CommandRequestId &&
                        r.OwnerId == request.OwnerId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Запрос с указанным ID не найден");

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
                    payload = JsonSerializer.Deserialize<TaskCreateData>(command.Payload, JsonHelper.Default);
                    break;
                }
            case CommandIntent.TaskQuery:
                {
                    payload = JsonSerializer.Deserialize<TaskQueryData>(command.Payload, JsonHelper.Default);
                    break;
                }
            case CommandIntent.TaskUpdate:
                {
                    payload = JsonSerializer.Deserialize<TaskUpdateData>(command.Payload, JsonHelper.Default);
                    break;
                }
            case CommandIntent.TaskDelete:
                {
                    payload = JsonSerializer.Deserialize<TaskDeleteData>(command.Payload, JsonHelper.Default);
                    break;
                }
            case CommandIntent.Ambiguous:
                {
                    payload = JsonSerializer.Deserialize<MessageData>(command.Payload, JsonHelper.Default);
                    break; 
                }
            case CommandIntent.Unknown:
                {
                    payload = JsonSerializer.Deserialize<MessageData>(command.Payload, JsonHelper.Default);
                    break;
                }
            default:
                {
                    payload = null;
                    break;
                }
        }

        return new GetVoiceTaskStatusResponse(command.Intent, payload);
    }
}
