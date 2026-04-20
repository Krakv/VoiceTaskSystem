using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class GetRuleTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Return_Rule()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();

        var rule = new RuleItem
        {
            RuleId = Guid.NewGuid(),
            OwnerId = userId,
            Event = RuleEvent.TaskCreated,
            Condition = JsonSerializer.Serialize(new ConditionGroup()),
            Action = JsonSerializer.Serialize(Array.Empty<RuleAction>()),
            IsActive = true
        };

        context.RuleItem.Add(rule);
        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetRuleQuery(userId, rule.RuleId));

        Assert.Equal(rule.RuleId, result.RuleId);
    }
}
