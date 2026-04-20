using MediatR;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.ToggleRule;

public sealed record ToggleRuleCommand(Guid OwnerId, Guid RuleId) : IRequest;
