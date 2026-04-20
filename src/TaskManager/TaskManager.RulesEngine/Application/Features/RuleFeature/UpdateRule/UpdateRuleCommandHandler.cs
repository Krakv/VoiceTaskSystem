using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.Shared.Exceptions;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.UpdateRule;

public sealed class UpdateRuleCommandHandler(AppDbContext dbContext, ILogger<UpdateRuleCommandHandler> logger, IRuleValidator validator) : IRequestHandler<UpdateRuleCommand>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<UpdateRuleCommandHandler> _logger = logger;
    private readonly IRuleValidator _validator = validator;

    public async Task Handle(UpdateRuleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateRuleCommand for RuleId: {RuleId}, UserId: {UserId}", request.RuleId, request.OwnerId);

        var ruleId = request.RuleId;

        var rule = await _dbContext.RuleItem
            .FirstOrDefaultAsync(r => r.RuleId == ruleId && r.OwnerId == request.OwnerId, cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Правило не найдено");

        _validator.Validate(request.RuleEvent, request.Conditions, request.Actions);

        rule.Event = request.RuleEvent;
        rule.Action = JsonSerializer.Serialize(request.Actions);
        rule.Condition = JsonSerializer.Serialize(request.Conditions);
        rule.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
