using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;

public sealed class GetRulesQueryHandler(AppDbContext dbContext, ILogger<GetRulesQueryHandler> logger, ICurrentUser user) : IRequestHandler<GetRulesQuery, GetRulesResponse>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<GetRulesQueryHandler> _logger = logger;
    private readonly ICurrentUser _user = user;

    public async Task<GetRulesResponse> Handle(GetRulesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling GetRulesQuery for UserId: {UserId}, RuleEvent: {RuleEvent}, IsActive: {IsActive}, Limit: {Limit}, Page: {Page}",
            _user.UserId, request.RuleEvent, request.IsActive, request.Limit, request.Page);

        var limit = int.TryParse(request.Limit, out var l) ? l : 20;
        var page = int.TryParse(request.Page, out var p) ? p : 0;

        var query = _dbContext.Set<RuleItem>()
            .AsNoTracking()
            .Where(r => r.OwnerId == _user.UserId);

        if (!string.IsNullOrWhiteSpace(request.RuleEvent) &&
            Enum.TryParse<RuleEvent>(request.RuleEvent, true, out var ruleEvent))
        {
            query = query.Where(r => r.Event == ruleEvent);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        // Пагинация
        var rules = await query
            .Skip(page * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Fetched {Count} rules for UserId: {UserId}", rules.Count, _user.UserId);

        return new GetRulesResponse(rules);
    }
}
