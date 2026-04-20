using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.ToggleRule;

public sealed class ToggleRuleCommandHandler(AppDbContext dbContext, ILogger<ToggleRuleCommandHandler> logger) : IRequestHandler<ToggleRuleCommand>
{
    private readonly ILogger<ToggleRuleCommandHandler> _logger = logger;
    private readonly AppDbContext _dbContext = dbContext;

    public async Task Handle(ToggleRuleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ToggleRuleCommand for RuleId: {RuleId}, UserId: {UserId}", request.RuleId, request.OwnerId);

        var ruleId = request.RuleId;

        var rule = await _dbContext.RuleItem
            .FirstOrDefaultAsync(r => r.RuleId == ruleId && r.OwnerId == request.OwnerId, cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Правило не найдено");

        rule.IsActive = !rule.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
