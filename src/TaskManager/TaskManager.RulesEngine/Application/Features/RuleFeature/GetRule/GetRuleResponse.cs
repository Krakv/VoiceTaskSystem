using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;

public sealed record GetRuleResponse(Guid RuleId,RuleEvent RuleEvent, ConditionGroup Condition, RuleAction[] Action, bool IsActive);
