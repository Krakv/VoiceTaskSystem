using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Models;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DTOs;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;
using TaskManager.TaskManagement.Application.Services.Interfaces;

namespace TaskManager.TaskManagement.Application.Services.VoiceProcessing;

public class VoiceProcessingHandler(
    AppDbContext db, 
    ISpeechProcessingClient speechClient, 
    ILogger<VoiceProcessingHandler> logger
    )
{
    private readonly AppDbContext _db = db;
    private readonly ISpeechProcessingClient _speechClient = speechClient;
    private readonly ILogger<VoiceProcessingHandler> _logger = logger;

    public async Task Handle(VoiceCommandCreationRequestedDto request)
    {
        _logger.LogInformation("Started to process voice command: {CommandRequestId}", request.VoiceCommandRequest.CommandId);

        var entity = await _db.CommandRequestItem.FindAsync(request.VoiceCommandRequest.CommandId);

        if (entity == null)
        {
            _logger.LogError("Voice Command was not found: {VoiceCommandRequest}", request);
            return;
        }

        try
        {
            // Начало обработки
            entity.Status = CommandRequestStatus.Processing;
            await _db.SaveChangesAsync();

            var result = await _speechClient.SendCommand(request.VoiceCommandRequest);

            if (result == null)
            {
                _logger.LogError("Voice Command was not processed: {VoiceCommandRequest}", request);
                entity.Status = CommandRequestStatus.Failed;
                await _db.SaveChangesAsync();
                return;
            }

            var payload = await FormPayloadAsync(result.CommandIntent, result.Entities, entity.OwnerId);

            // Команда обработана
            entity.Intent = result.CommandIntent;
            entity.Payload = payload;
            entity.Status = result.CommandIntent == CommandIntent.TaskQuery 
                ? CommandRequestStatus.Accepted 
                : CommandRequestStatus.WaitingForConfirmation;
            _logger.LogInformation("Voice command successfully processed: {CommandRequestId}", request.VoiceCommandRequest.CommandId);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while processing voice command: {VoiceCommandRequest}", request);
            entity.Status = CommandRequestStatus.Failed;
        }
        finally
        {
            await _db.SaveChangesAsync();
        }
    }

    public static GetVoiceTaskStatusResponse BuildResponse(
        CommandIntent intent,
        object? payloadData = null)
    {
        IVoiceTaskPayload? payload = intent switch
        {
            CommandIntent.TaskCreate => payloadData as TaskCreateData,
            CommandIntent.TaskUpdate => payloadData as TaskUpdateData,
            CommandIntent.TaskDelete => payloadData as TaskDeleteData,
            CommandIntent.TaskQuery => payloadData as TaskQueryData,
            CommandIntent.Ambiguous => payloadData as MessageData,
            CommandIntent.Unknown => payloadData as MessageData,
            _ => null
        };

        return new GetVoiceTaskStatusResponse(intent, payload);
    }

    private async Task<string> FormPayloadAsync(CommandIntent intent, TaskItemModel taskItemModel, Guid userId)
    {
        switch (intent)
        {
            case CommandIntent.TaskCreate:
                {
                    return JsonSerializer.Serialize(await ToTaskCreateData(taskItemModel, userId, "Текст распознан"));
                }
            case CommandIntent.TaskUpdate:
                {
                    return JsonSerializer.Serialize(await ToTaskUpdateData(taskItemModel, userId));
                }
            case CommandIntent.TaskDelete:
                {
                    return JsonSerializer.Serialize(await ToTaskDeleteData(taskItemModel, userId));
                }
            case CommandIntent.TaskQuery:
                {
                    return JsonSerializer.Serialize(await ToTaskQueryDataAsync(taskItemModel, userId));
                }
            case CommandIntent.Ambiguous:
                {
                    return JsonSerializer.Serialize(new MessageData("Намерение неоднозначно, требуется уточнение"));
                }
            case CommandIntent.Unknown:
                {
                    return JsonSerializer.Serialize(new MessageData("Не удалось определить намерение пользователя"));
                }
        }
        throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Выявлено некорректное значение намерения");
    }

    public async Task<TaskCreateData> ToTaskCreateData(
        TaskItemModel model,
        Guid userId,
        string message = "",
        bool confirmationRequired = true)
    {
        return new TaskCreateData(
            ProjectName: model.ProjectName,
            Title: model.Title ?? string.Empty,
            Description: model.Description,
            Status: model.Status?.ToString() ?? TaskItemStatus.New.ToString(),
            Priority: model.Priority?.ToString() ?? TaskItemPriority.Low.ToString(),
            DueDate: model.DueDate,
            Message: message,
            ParentTaskId: await FindParentTaskAsync(model.ParentTaskName, userId),
            ConfirmationRequired: confirmationRequired
        );
    }

    public async Task<TaskUpdateData> ToTaskUpdateData(
        TaskItemModel model,
        Guid userId,
        bool confirmationRequired = true)
    {
        return new TaskUpdateData(
            TaskIds: await FindTasksAsync(model.Title, userId),
            ProjectName: model.ProjectName,
            Description: model.Description,
            Status: model.Status?.ToString() ?? TaskItemStatus.New.ToString(),
            Priority: model.Priority?.ToString() ?? TaskItemPriority.Low.ToString(),
            DueDate: model.DueDate,
            ParentTaskId: await FindParentTaskAsync(model.ParentTaskName, userId),
            ConfirmationRequired: confirmationRequired
        );
    }

    public async Task<TaskQueryData> ToTaskQueryDataAsync(TaskItemModel model, Guid userId)
    {
        var tasks = await FindTasksByProjectNames(model.ProjectName, userId);

        return new TaskQueryData([.. tasks.Select(t => ToTaskData(t))]);
    }

    public static TaskData ToTaskData(TaskItem model)
    {
        return new TaskData(
            model.TaskId,
            model.ProjectName ?? string.Empty,
            model.Title,
            model.Description ?? string.Empty,
            model.Status.ToString(),
            model.Priority.ToString(),
            model.ParentTaskId ?? Guid.Empty,
            model.DueDate,
            model.CreatedAt,
            model.UpdatedAt
        );
    }

    public async Task<TaskDeleteData> ToTaskDeleteData(
        TaskItemModel model,
        Guid userId,
        bool confirmationRequired = true)
    {
        return new TaskDeleteData(
            TaskIds: await FindTasksAsync(model.Title, userId)
        );
    }

    private async Task<Guid?> FindParentTaskAsync(string? parentTaskName, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(parentTaskName))
            return null;

        var tasks = await _db.TaskItems
            .Where(t => t.OwnerId == userId)
            .Select(t => new { t.TaskId, t.Title })
            .ToListAsync();

        var bestMatch = tasks
            .Select(t => new
            {
                t.TaskId,
                Score = FuzzySharp.Fuzz.PartialRatio(parentTaskName, t.Title)
            })
            .Where(x => x.Score > 70)
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        return bestMatch?.TaskId;
    }

    private async Task<List<Guid>> FindTasksAsync(string? taskName, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(taskName))
            return [];

        var tasks = await _db.TaskItems
            .Where(t => t.OwnerId == userId)
            .Select(t => new { t.TaskId, t.Title })
            .ToListAsync();

        return tasks
            .Select(t => new
            {
                t.TaskId,
                Score = FuzzySharp.Fuzz.TokenSetRatio(taskName, t.Title)
            })
            .Where(x => x.Score > 65)
            .OrderByDescending(x => x.Score)
            .Take(5)
            .Select(x => x.TaskId)
            .ToList();
    }

    private async Task<List<TaskItem>> FindTasksByProjectNames(string? projectName, Guid userId)
    {
        var query = _db.TaskItems.Where(e => e.OwnerId == userId);

        var tasks = await query
            .Select(t => new
            {
                Task = t,
                t.ProjectName
            })
            .ToListAsync();

        if (string.IsNullOrWhiteSpace(projectName))
        {
            return tasks
                .Select(x => x.Task)
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        return tasks
            .Select(x => new
            {
                x.Task,
                Score = FuzzySharp.Fuzz.Ratio(projectName, x.ProjectName ?? "")
            })
            .Where(x => x.Score > 70)
            .OrderByDescending(x => x.Score)
            .Select(x => x.Task)
            .OrderBy(t => t.DueDate)
            .ToList();
    }
}
