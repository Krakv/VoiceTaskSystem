using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Interfaces;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;

public sealed class CreateRuleCommandHandler(AppDbContext dbContext, ILogger<CreateRuleCommandHandler> logger, ICurrentUser user, IRuleValidator validator) : IRequestHandler<CreateRuleCommand, CreateRuleResponse>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<CreateRuleCommandHandler> _logger = logger;
    private readonly ICurrentUser _user = user;
    private readonly IRuleValidator _validator = validator;

    public async Task<CreateRuleResponse> Handle(CreateRuleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateRuleCommand for RuleEvent: {RuleEvent}", request.RuleEvent);

        _validator.Validate(request.RuleEvent, request.Conditions, request.Actions);

        var newRule = new RuleItem
        {
            RuleId = Guid.NewGuid(),
            OwnerId = _user.UserId,
            Event = request.RuleEvent,
            Condition = JsonSerializer.Serialize(request.Conditions),
            Action = JsonSerializer.Serialize(request.Actions),
            IsActive = request.IsActive
        };

        await _dbContext.RuleItem.AddAsync(newRule, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created Rule: {RuleItem}", JsonSerializer.Serialize(newRule));
        return new CreateRuleResponse(newRule.RuleId);
    }
}
