using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.DeleteRule;

public sealed class DeleteRuleCommandHandler(AppDbContext dbContext, ILogger<DeleteRuleCommandHandler> logger) : IRequestHandler<DeleteRuleCommand>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<DeleteRuleCommandHandler> _logger = logger;

    public async Task Handle(DeleteRuleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteRuleCommand for UserId: {UserId}, RuleId: {RuleId}", request.OwnerId, request.RuleId);

        var ruleId = request.RuleId;

        var rule = await _dbContext.RuleItem
            .Where(r => r.RuleId == ruleId && 
                        r.OwnerId == request.OwnerId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Правило не найдено");

        _dbContext.RuleItem.Remove(rule);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Successfully deleted RuleId: {RuleId} for UserId: {UserId}", ruleId, request.OwnerId);
    }
}
