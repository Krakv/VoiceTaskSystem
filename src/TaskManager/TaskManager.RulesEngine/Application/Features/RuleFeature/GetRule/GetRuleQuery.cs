using MediatR;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;

public sealed record GetRuleQuery(string RuleId) : IRequest<GetRuleResponse>;
