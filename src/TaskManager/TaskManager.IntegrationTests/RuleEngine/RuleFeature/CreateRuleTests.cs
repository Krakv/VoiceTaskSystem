using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class CreateRuleTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public CreateRuleTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Create_Rule()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var command = new CreateRuleCommand(
            RuleEvent.TaskCreated,
            new ConditionGroup(),
            Array.Empty<RuleAction>(),
            true
        );

        var response = await mediator.Send(command);

        var rule = await context.RuleItem.FindAsync(response.RuleId);

        Assert.NotNull(rule);
        Assert.Equal(user.UserId, rule.OwnerId);
        Assert.True(rule.IsActive);
    }
}
