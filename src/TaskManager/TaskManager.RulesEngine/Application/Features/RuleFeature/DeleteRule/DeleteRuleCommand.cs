using MediatR;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.DeleteRule;

public sealed record DeleteRuleCommand(string RuleId) : IRequest;
