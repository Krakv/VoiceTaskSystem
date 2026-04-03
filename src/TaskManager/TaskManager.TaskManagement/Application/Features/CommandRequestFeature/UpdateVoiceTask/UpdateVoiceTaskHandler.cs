using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.UpdateVoiceTask;

public sealed class UpdateVoiceTaskHandler : IRequestHandler<UpdateVoiceTaskCommand, UpdateVoiceTaskResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UpdateVoiceTaskHandler> _logger;

    public UpdateVoiceTaskHandler(AppDbContext dbContext, ILogger<UpdateVoiceTaskHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UpdateVoiceTaskResponse> Handle(UpdateVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        var command = await _dbContext.CommandRequestItem.FindAsync(Guid.Parse(request.CommandRequestId));
        if (command == null)
            throw new ValidationAppException("NOT_FOUND", "Команда не найдена");

        if (command.Payload == null)
            throw new ValidationAppException("INVALID_STATE", "Команда не содержит payload");

        _logger.LogInformation("Updating CommandRequest payload: {CommandRequestId}", request.CommandRequestId);

        Dictionary<string, string> updatedFields = request.TaskId != null
            ? new Dictionary<string, string> { [nameof(request.TaskId)] = request.TaskId! }
            : new Dictionary<string, string>();

        switch (command.Intent)
        {
            case CommandIntent.TaskCreate:
                {
                    var payload = JsonSerializer.Deserialize<TaskCreateData>(command.Payload)
                                  ?? throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Не удалось десериализовать payload");
                    (payload, updatedFields) = UpdateTaskCreatePayload(payload, request, updatedFields);
                    command.Payload = JsonSerializer.Serialize(payload);
                    break;
                }

            case CommandIntent.TaskUpdate:
                {
                    var payload = JsonSerializer.Deserialize<TaskUpdateData>(command.Payload)
                                  ?? throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Не удалось десериализовать payload");
                    (payload, updatedFields) = UpdateTaskUpdatePayload(payload, request, updatedFields);
                    command.Payload = JsonSerializer.Serialize(payload);
                    break;
                }

            case CommandIntent.TaskDelete:
                {
                    var payload = JsonSerializer.Deserialize<TaskDeleteData>(command.Payload)
                                  ?? throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Не удалось десериализовать payload");
                    (payload, updatedFields) = UpdateTaskDeletePayload(payload, request, updatedFields);
                    command.Payload = JsonSerializer.Serialize(payload);
                    break;
                }

            default:
                throw new ValidationAppException("INVALID_INTENT", "Команда не поддерживает редактирование");
        }

        command.UpdatedAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("CommandRequest payload updated: {CommandRequestId}", request.CommandRequestId);

        return new UpdateVoiceTaskResponse(updatedFields);
    }

    private static (TaskCreateData, Dictionary<string, string>) UpdateTaskCreatePayload(
        TaskCreateData payload, UpdateVoiceTaskCommand request, Dictionary<string, string> updatedFields)
    {
        void CheckUpdate<T>(string name, T? oldValue, T? newValue)
        {
            if (!object.Equals(newValue, default(T)) && !EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                updatedFields[name] = newValue!.ToString()!;
            }
        }

        CheckUpdate(nameof(payload.ParentTaskId), payload.ParentTaskId.ToString(), request.ParentTaskId);
        CheckUpdate(nameof(payload.ProjectName), payload.ProjectName, request.ProjectName);
        CheckUpdate(nameof(payload.Title), payload.Title, request.Title);
        CheckUpdate(nameof(payload.Description), payload.Description, request.Description);
        CheckUpdate(nameof(payload.Status), payload.Status, request.Status);
        CheckUpdate(nameof(payload.Priority), payload.Priority, request.Priority);
        CheckUpdate(nameof(payload.DueDate), payload.DueDate?.ToString("o"), request.DueDate);

        Guid? parentTaskId;

        if (request.ParentTaskId != null)
        {
            parentTaskId = request.ParentTaskId == string.Empty
                ? null
                : Guid.Parse(request.ParentTaskId);
        }
        else
        {
            parentTaskId = payload.ParentTaskId;
        }

        var updatedPayload = payload with
        {
            ProjectName = request.ProjectName ?? payload.ProjectName,
            Title = request.Title ?? payload.Title,
            Description = request.Description ?? payload.Description,
            Status = request.Status ?? payload.Status,
            Priority = request.Priority ?? payload.Priority,
            DueDate = request.DueDate != null ? DateTimeOffset.Parse(request.DueDate, CultureInfo.InvariantCulture) : payload.DueDate,
            ParentTaskId = parentTaskId
        };

        return (updatedPayload, updatedFields);
    }

    private static (TaskUpdateData, Dictionary<string, string>) UpdateTaskUpdatePayload(
        TaskUpdateData payload, UpdateVoiceTaskCommand request, Dictionary<string, string> updatedFields)
    {
        void CheckUpdate<T>(string name, T? oldValue, T? newValue)
        {
            if (!object.Equals(newValue, default(T)) && !EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                updatedFields[name] = newValue!.ToString()!;
            }
        }

        var newTaskIds = !string.IsNullOrWhiteSpace(request.TaskId)
            ? new List<Guid> { Guid.Parse(request.TaskId) }
            : payload.TaskIds;

        if (!newTaskIds.SequenceEqual(payload.TaskIds))
            updatedFields[nameof(payload.TaskIds)] = string.Join(",", newTaskIds);

        CheckUpdate(nameof(payload.ParentTaskId), payload.ParentTaskId.ToString(), request.ParentTaskId);
        CheckUpdate(nameof(payload.ProjectName), payload.ProjectName, request.ProjectName);
        CheckUpdate(nameof(payload.Description), payload.Description, request.Description);
        CheckUpdate(nameof(payload.Status), payload.Status, request.Status);
        CheckUpdate(nameof(payload.Priority), payload.Priority, request.Priority);
        CheckUpdate(nameof(payload.DueDate), payload.DueDate?.ToString("o"), request.DueDate);

        Guid? parentTaskId;

        if (request.ParentTaskId != null)
        {
            parentTaskId = request.ParentTaskId == string.Empty
                ? null
                : Guid.Parse(request.ParentTaskId);
        }
        else
        {
            parentTaskId = payload.ParentTaskId;
        }

        var updatedPayload = payload with
        {
            TaskIds = newTaskIds,
            ProjectName = request.ProjectName ?? payload.ProjectName,
            Description = request.Description ?? payload.Description,
            Status = request.Status ?? payload.Status,
            Priority = request.Priority ?? payload.Priority,
            DueDate = request.DueDate != null
                ? DateTimeOffset.Parse(request.DueDate, CultureInfo.InvariantCulture)
                : payload.DueDate,
            ParentTaskId = parentTaskId
        };

        return (updatedPayload, updatedFields);
    }

    private static (TaskDeleteData, Dictionary<string, string>) UpdateTaskDeletePayload(
        TaskDeleteData payload, UpdateVoiceTaskCommand request, Dictionary<string, string> updatedFields)
    {
        var newTaskIds = !string.IsNullOrWhiteSpace(request.TaskId)
            ? new List<Guid> { Guid.Parse(request.TaskId) }
            : payload.TaskIds;

        if (!newTaskIds.SequenceEqual(payload.TaskIds))
            updatedFields[nameof(payload.TaskIds)] = string.Join(",", newTaskIds);

        var updatedPayload = payload with
        {
            TaskIds = newTaskIds
        };

        return (updatedPayload, updatedFields);
    }
}