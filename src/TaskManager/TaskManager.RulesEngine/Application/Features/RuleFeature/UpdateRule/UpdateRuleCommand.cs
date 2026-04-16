using MediatR;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.UpdateRule;

public sealed record UpdateRuleCommand(
    string RuleId,
    RuleEvent RuleEvent,
    ConditionGroup? Conditions,
    IEnumerable<RuleAction> Actions,
    bool IsActive = true
    ) : IRequest;
