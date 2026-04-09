using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.ToggleRule;

public sealed class ToggleRuleCommandHandler(AppDbContext dbContext, ILogger<ToggleRuleCommandHandler> logger, ICurrentUser user) : IRequestHandler<ToggleRuleCommand>
{
    private readonly ILogger<ToggleRuleCommandHandler> _logger = logger;
    private readonly ICurrentUser _user = user;
    private readonly AppDbContext _dbContext = dbContext;

    public async Task Handle(ToggleRuleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ToggleRuleCommand for RuleId: {RuleId}, UserId: {UserId}", request.RuleId, _user.UserId);

        var ruleId = Guid.Parse(request.RuleId);

        var rule = await _dbContext.RuleItem.FindAsync([ruleId], cancellationToken: cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Правило не найдено");

        if (_user.UserId != rule.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        rule.IsActive = !rule.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return;
    }
}
