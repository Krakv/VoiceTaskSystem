using TaskManager.Shared.Domain.Entities;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;

public sealed record GetRulesResponse(List<RuleItem> Rules);
