using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.UpdateRule;

public sealed class UpdateRuleCommandHandler(AppDbContext dbContext, ILogger<UpdateRuleCommandHandler> logger, ICurrentUser user, IRuleValidator validator) : IRequestHandler<UpdateRuleCommand>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<UpdateRuleCommandHandler> _logger = logger;
    private readonly ICurrentUser _user = user;
    private readonly IRuleValidator _validator = validator;

    public async Task Handle(UpdateRuleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateRuleCommand for RuleId: {RuleId}, UserId: {UserId}", request.RuleId, _user.UserId);

        var ruleId = Guid.Parse(request.RuleId);

        var rule = await _dbContext.RuleItem.FindAsync([ruleId], cancellationToken: cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Правило не найдено");

        if (_user.UserId != rule.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        _validator.Validate(request.RuleEvent, request.Conditions, request.Actions);

        rule.Event = request.RuleEvent;
        rule.Action = JsonSerializer.Serialize(request.Actions);
        rule.Condition = JsonSerializer.Serialize(request.Conditions);
        rule.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return;
    }
}
