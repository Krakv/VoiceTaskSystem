using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Interfaces;
using TaskManager.Shared.Exceptions;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;

public sealed class GetRuleQueryHandler(AppDbContext dbContext, ILogger<GetRuleQueryHandler> logger, ICurrentUser user) : IRequestHandler<GetRuleQuery, GetRuleResponse>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<GetRuleQueryHandler> _logger = logger;
    private readonly ICurrentUser _user = user;

    public async Task<GetRuleResponse> Handle(GetRuleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetRuleQuery for UserId: {UserId}, RuleId: {RuleId}", _user.UserId, request.RuleId);

        var ruleId = Guid.Parse(request.RuleId);

        var rule = await _dbContext.RuleItem
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RuleId == ruleId && r.OwnerId == _user.UserId, cancellationToken);

        if (rule is null)
        {
            _logger.LogWarning("Rule not found or access denied. RuleId: {RuleId}, UserId: {UserId}", ruleId, _user.UserId);

            throw new ValidationAppException("NOT_FOUND", "Правило не найдено");
        }

        _logger.LogInformation("Successfully fetched RuleId: {RuleId} for UserId: {UserId}", ruleId, _user.UserId);

        return new GetRuleResponse(rule);
    }
}
