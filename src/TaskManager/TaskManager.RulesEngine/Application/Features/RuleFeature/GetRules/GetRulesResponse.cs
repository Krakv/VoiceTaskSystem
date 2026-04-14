using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;

public sealed record GetRulesResponse(List<GetRuleResponse> Rules);
