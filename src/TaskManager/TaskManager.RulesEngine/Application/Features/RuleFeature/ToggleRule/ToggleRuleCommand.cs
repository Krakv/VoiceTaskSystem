using MediatR;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.ToggleRule;

public sealed record ToggleRuleCommand(string RuleId) : IRequest;
