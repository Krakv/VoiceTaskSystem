using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Utils;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;

public sealed class GetRulesQueryHandler(AppDbContext dbContext, ILogger<GetRulesQueryHandler> logger) : IRequestHandler<GetRulesQuery, GetRulesResponse>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<GetRulesQueryHandler> _logger = logger;

    public async Task<GetRulesResponse> Handle(GetRulesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling GetRulesQuery for UserId: {UserId}, RuleEvent: {RuleEvent}, IsActive: {IsActive}, Limit: {Limit}, Page: {Page}",
            request.OwnerId, request.RuleEvent, request.IsActive, request.Limit, request.Page);

        var limit = request.Limit;
        var page = request.Page;

        var query = _dbContext.Set<RuleItem>()
            .AsNoTracking()
            .Where(r => r.OwnerId == request.OwnerId);

        if (request.RuleEvent != null)
        {
            query = query.Where(r => r.Event == request.RuleEvent);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        // Пагинация
        var rulesRaw = await query
            .Skip(page * limit)
            .Take(limit)
            .Select(rule => new
            {
                rule.RuleId,
                rule.Event,
                rule.Condition,
                rule.Action,
                rule.IsActive
            })
            .ToListAsync(cancellationToken);

        var rules = rulesRaw.Select(rule =>
        {
            var conditionGroup = rule.Condition == null ? null : JsonSerializer.Deserialize<ConditionGroup>(rule.Condition, JsonHelper.Default);
            var actions = JsonSerializer.Deserialize<RuleAction[]>(rule.Action, JsonHelper.Default);

            if (conditionGroup == null || actions == null)
                throw new ValidationAppException("INTERNAL_SERVER_ERROR", "Ошибка десериализации");

            return new GetRuleResponse(rule.RuleId, rule.Event, conditionGroup, actions, rule.IsActive);
        }).ToList();

        _logger.LogInformation("Fetched {Count} rules for UserId: {UserId}", rules.Count, request.OwnerId);

        return new GetRulesResponse(rules);
    }
}
