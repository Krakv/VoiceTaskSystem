using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DeleteVoiceTask;

public sealed class DeleteVoiceTaskHandler(AppDbContext dbContext, ILogger<DeleteVoiceTaskHandler> logger) : IRequestHandler<DeleteVoiceTaskCommand, DeleteVoiceTaskResponse>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<DeleteVoiceTaskHandler> _logger = logger;
    public async Task<DeleteVoiceTaskResponse> Handle(DeleteVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Started to check status: {CommandRequestId} command", request.CommandRequestId);
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

        if (command.Status == CommandRequestStatus.Failed)
        {
            throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Не удалось обработать команду");
        }

        command.CancelledAt = DateTime.UtcNow;
        command.Status = CommandRequestStatus.Cancelled;
        await _dbContext.SaveChangesAsync();
        _logger.LogError("Cancelled: {CommandRequestId}", request.CommandRequestId);
        return new DeleteVoiceTaskResponse(request.CommandRequestId);
    }
}
