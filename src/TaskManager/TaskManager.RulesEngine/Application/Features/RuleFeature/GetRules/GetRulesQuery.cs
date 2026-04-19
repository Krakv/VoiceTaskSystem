using MediatR;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;

public sealed record GetRulesQuery(
    Guid OwnerId,
    RuleEvent? RuleEvent = null,
    bool? IsActive = null,
    int Limit = 20,
    int Page = 0
    ) : IRequest<GetRulesResponse>;
