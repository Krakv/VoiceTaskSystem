using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.UpdateRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class UpdateRuleTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public UpdateRuleTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Update_Rule()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var rule = new RuleItem
        {
            RuleId = Guid.NewGuid(),
            OwnerId = user.UserId,
            Event = RuleEvent.TaskCreated,
            Condition = "{}",
            Action = "[]",
            IsActive = true
        };

        context.RuleItem.Add(rule);
        await context.SaveChangesAsync();

        await mediator.Send(new UpdateRuleCommand(
            rule.RuleId.ToString(),
            RuleEvent.TaskDeleted,
            new ConditionGroup(),
            Array.Empty<RuleAction>(),
            false
        ));

        var updated = await context.RuleItem.FindAsync(rule.RuleId);

        Assert.Equal(RuleEvent.TaskDeleted, updated.Event);
        Assert.False(updated.IsActive);
    }
}
