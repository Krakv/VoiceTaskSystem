using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.UpdateRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class UpdateRuleTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Update_Rule()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();

        var rule = new RuleItem
        {
            RuleId = Guid.NewGuid(),
            OwnerId = userId,
            Event = RuleEvent.TaskCreated,
            Condition = "{}",
            Action = "[]",
            IsActive = true
        };

        context.RuleItem.Add(rule);
        await context.SaveChangesAsync();

        await mediator.Send(new UpdateRuleCommand(
            userId,
            rule.RuleId,
            RuleEvent.TaskDeleted,
            new ConditionGroup(),
            Array.Empty<RuleAction>(),
            false
        ));

        var updated = await context.RuleItem.FindAsync(rule.RuleId);

        Assert.NotNull(updated);
        Assert.Equal(RuleEvent.TaskDeleted, updated.Event);
        Assert.False(updated.IsActive);
    }
}
