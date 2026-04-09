using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Events;

namespace TaskManager.RulesEngine.Application.Features.EventHandlers;

public sealed class CreateTaskEventHandler(AppDbContext dbContext, IRuleApplier ruleApplier, ILogger<CreateTaskEventHandler> logger) : INotificationHandler<TaskCreatedEvent>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IRuleApplier _ruleApplier = ruleApplier;
    private readonly ILogger<CreateTaskEventHandler> _logger = logger;
    public async Task Handle(TaskCreatedEvent notification, CancellationToken cancellationToken)
    {
        var task = await _dbContext.TaskItems.FindAsync([notification.TaskId], cancellationToken);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found for {Event}. UserId: {UserId}", notification.TaskId, notification.Event, notification.UserId);
            return;
        }

        try
        {
            await _ruleApplier.ApplyRulesAsync(
                task,
                RuleEvent.TaskCreated,
                notification.UserId,
                cancellationToken
            );
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying rules for {Event}. TaskId: {TaskId}, UserId: {UserId}", notification.Event, notification.TaskId, notification.UserId);
        }
    }
}
