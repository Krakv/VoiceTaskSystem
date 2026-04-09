using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.ApiGateway.DTOs;

public sealed record UpdateRuleDto(
    RuleEvent RuleEvent,
    ConditionGroup Conditions,
    IEnumerable<RuleAction> Actions,
    bool IsActive = true
    );
