using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.ConfirmVoiceTask;

public sealed class ConfirmVoiceTaskHandler : IRequestHandler<ConfirmVoiceTaskCommand, ConfirmVoiceTaskResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ConfirmVoiceTaskHandler> _logger;
    private readonly IMediator _mediator;

    public ConfirmVoiceTaskHandler(
        AppDbContext dbContext,
        ILogger<ConfirmVoiceTaskHandler> logger,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<ConfirmVoiceTaskResponse> Handle(ConfirmVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogError("Started to confirm: {CommandRequestId}", request.CommandRequestId);

        var command = await _dbContext.CommandRequestItem
            .Where(r => r.CommandRequestId == request.CommandRequestId &&
                        r.OwnerId == request.OwnerId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Запрос с указанным ID не найден");

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

        try
        {
            IVoiceTaskPayload? payload = command.Intent switch
            {
                CommandIntent.TaskCreate => JsonSerializer.Deserialize<TaskCreateData>(command.Payload ?? ""),
                CommandIntent.TaskUpdate => JsonSerializer.Deserialize<TaskUpdateData>(command.Payload ?? ""),
                CommandIntent.TaskDelete => JsonSerializer.Deserialize<TaskDeleteData>(command.Payload ?? ""),
                _ => null
            };

            if (payload == null)
                throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Не удалось прочитать payload команды");

            string? changedTaskId = null;

            switch (command.Intent)
            {
                case CommandIntent.TaskCreate:
                    {
                        var createData = (payload as TaskCreateData)!;
                        var resp = await _mediator.Send(new CreateTaskCommand(
                            command.OwnerId,
                            createData.ProjectName,
                            createData.Title,
                            createData.Description,
                            createData.Status,
                            createData.Priority,
                            createData.DueDate,
                            createData.ParentTaskId
                        ), cancellationToken);
                        changedTaskId = resp.ToString();
                        break;
                    }

                case CommandIntent.TaskUpdate:
                    {
                        var updateData = (payload as TaskUpdateData)!;
                        Guid taskId = (Guid)updateData.Tasks.FirstOrDefault()?.TaskId!;

                        var resp = await _mediator.Send(new UpdateTaskPatchCommand(
                                command.OwnerId,
                                taskId,
                                updateData.ProjectName,
                                null,
                                updateData.Description,
                                updateData.Status,
                                updateData.Priority,
                                updateData.DueDate?.ToString(),
                                updateData.ParentTaskId?.ToString()
                            ), cancellationToken);
                        changedTaskId = resp.ToString();
                        break;
                    }
                    

                case CommandIntent.TaskDelete:
                    {
                        var deleteData = (payload as TaskDeleteData)!;
                        var resp = await _mediator.Send(new DeleteTaskCommand(command.OwnerId, (Guid)deleteData.Tasks.FirstOrDefault()?.TaskId!), cancellationToken);
                        changedTaskId = resp.ToString();
                        break;
                    }
            }

            command.Status = CommandRequestStatus.Accepted;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogError("Confirmed: {CommandRequestId}", request.CommandRequestId);

            return new ConfirmVoiceTaskResponse
            (
                changedTaskId,
                command.Intent.ToString()
            );
        }
        catch (Exception)
        {
            command.Status = CommandRequestStatus.Failed;
            await _dbContext.SaveChangesAsync(cancellationToken);
            throw;
        }
    }
}