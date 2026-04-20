using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class CreateRuleTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Create_Rule()
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

        var response = await mediator.Send(command);

        var rule = await context.RuleItem.FindAsync(response.RuleId);

        Assert.NotNull(rule);
        Assert.Equal(userId, rule.OwnerId);
        Assert.True(rule.IsActive);
    }
}
