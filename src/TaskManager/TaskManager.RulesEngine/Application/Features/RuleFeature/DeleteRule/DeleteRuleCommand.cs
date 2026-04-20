using MediatR;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.DeleteRule;

public sealed record DeleteRuleCommand(Guid OwnerId, Guid RuleId) : IRequest;
