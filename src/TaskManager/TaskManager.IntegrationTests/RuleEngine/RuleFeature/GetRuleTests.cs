using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class GetRuleTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public GetRuleTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Return_Rule()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var rule = new RuleItem
        {
            RuleId = Guid.NewGuid(),
            OwnerId = user.UserId,
            Event = RuleEvent.TaskCreated,
            Condition = JsonSerializer.Serialize(new ConditionGroup()),
            Action = JsonSerializer.Serialize(Array.Empty<RuleAction>()),
            IsActive = true
        };

        context.RuleItem.Add(rule);
        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetRuleQuery(rule.RuleId.ToString()));

        Assert.Equal(rule.RuleId, result.RuleId);
    }
}
