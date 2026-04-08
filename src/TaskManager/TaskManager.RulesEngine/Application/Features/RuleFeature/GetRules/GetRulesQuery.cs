using MediatR;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;

public sealed record GetRulesQuery(
    string? RuleEvent = null,
    bool? IsActive = null,
    string? Limit = "20",
    string? Page = "0"
    ) : IRequest<GetRulesResponse>;
