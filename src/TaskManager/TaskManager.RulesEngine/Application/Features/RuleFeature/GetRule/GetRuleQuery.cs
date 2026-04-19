using MediatR;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;

public sealed record GetRuleQuery(Guid OwnerId, Guid RuleId) : IRequest<GetRuleResponse>;
