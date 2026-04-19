using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.DeleteRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class DeleteRuleTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Delete_Rule()
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

        await mediator.Send(new DeleteRuleCommand(userId, ruleId));

        var deleted = await context.RuleItem.FindAsync(ruleId);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task Should_Throw_When_Not_Owner()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var userId = await fixture.CreateUserAsync();

        var command = new CreateRuleCommand(
            userId,
            RuleEvent.TaskCreated,
            new ConditionGroup(),
            Array.Empty<RuleAction>(),
            true
        );

        var ruleId = (await mediator.Send(command)).RuleId;

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new DeleteRuleCommand(Guid.NewGuid(), ruleId)));
    }
}
