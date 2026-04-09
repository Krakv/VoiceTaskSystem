using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.DeleteRule;

public sealed class DeleteRuleCommandHandler(AppDbContext dbContext, ILogger<DeleteRuleCommandHandler> logger, ICurrentUser user) : IRequestHandler<DeleteRuleCommand>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<DeleteRuleCommandHandler> _logger = logger;
    private readonly ICurrentUser _user = user;

    public async Task Handle(DeleteRuleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteRuleCommand for UserId: {UserId}, RuleId: {RuleId}", _user.UserId, request.RuleId);

        var ruleId = Guid.Parse(request.RuleId);

        var rule = await _dbContext.RuleItem.FindAsync([ruleId], cancellationToken: cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Правило не найдено");

        if (_user.UserId != rule.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        _dbContext.RuleItem.Remove(rule);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Successfully deleted RuleId: {RuleId} for UserId: {UserId}", ruleId, _user.UserId);

        return;
    }
}
