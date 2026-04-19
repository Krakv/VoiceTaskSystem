using MediatR;
using TaskManager.Shared.Exceptions;
using TaskManager.Repository.Context;
using Microsoft.Extensions.Logging;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;

public sealed class DeleteTaskHandler(AppDbContext context, ILogger<DeleteTaskHandler> logger) : IRequestHandler<DeleteTaskCommand, Guid>
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<DeleteTaskHandler> _logger = logger;

    public async Task<Guid> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems.FindAsync([request.TaskId], cancellationToken: cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (request.OwnerId != task.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted task with id {TaskId}", task.TaskId);
        return request.TaskId;
    }
}
