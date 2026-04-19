using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.ToggleRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class ToggleRuleTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Toggle_Rule()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();

        var command = new CreateRuleCommand(
            userId,
            RuleEvent.TaskCreated,
            new ConditionGroup(),
            Array.Empty<RuleAction>(),
            true
        );

        var ruleId = (await mediator.Send(command)).RuleId;

        await mediator.Send(new ToggleRuleCommand(userId, ruleId));

        var updated = await context.RuleItem.FindAsync(ruleId);

        Assert.NotNull(updated);
        Assert.False(updated.IsActive);
    }
}
