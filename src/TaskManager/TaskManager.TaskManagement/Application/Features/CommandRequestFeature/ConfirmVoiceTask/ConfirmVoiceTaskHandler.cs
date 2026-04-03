using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;

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
        var command = await _dbContext.CommandRequestItem.FindAsync(Guid.Parse(request.CommandRequestId));
        _logger.LogError("Started to confirm: {CommandRequestId}", request.CommandRequestId);

        if (command == null || command.Status != CommandRequestStatus.WaitingForConfirmation)
        {
            throw new ValidationAppException("NOT_FOUND", "Нет команды для подтверждения или она уже обработана");
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
                            createData.ProjectName ?? "",
                            createData.Title,
                            createData.Description ?? "",
                            createData.Status,
                            createData.Priority,
                            createData.DueDate?.ToString() ?? "",
                            createData.ParentTaskId?.ToString() ?? ""
                        ), cancellationToken);
                        changedTaskId = resp;
                        break;
                    }

                case CommandIntent.TaskUpdate:
                    {
                        var updateData = (payload as TaskUpdateData)!;
                        var resp = await _mediator.Send(new UpdateTaskCommand(
                                updateData.TaskIds.FirstOrDefault().ToString(),
                                updateData.ProjectName ?? "",
                                "",
                                updateData.Description ?? "",
                                updateData.Status ?? "",
                                updateData.Priority ?? "",
                                updateData.DueDate?.ToString() ?? "",
                                updateData.ParentTaskId?.ToString()
                            ), cancellationToken);
                        changedTaskId = resp;
                        break;
                    }
                    

                case CommandIntent.TaskDelete:
                    {
                        var deleteData = (payload as TaskDeleteData)!;
                        var resp = await _mediator.Send(new DeleteTaskCommand(deleteData.TaskIds.FirstOrDefault().ToString()), cancellationToken);
                        changedTaskId = resp;
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