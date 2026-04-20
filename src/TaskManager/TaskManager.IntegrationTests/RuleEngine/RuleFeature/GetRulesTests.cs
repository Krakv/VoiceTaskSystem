using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class GetRulesTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Filter_By_Event()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var ownerId1 = await fixture.CreateUserAsync();
        var ownerId2 = await fixture.CreateUserAsync("test2");

        context.RuleItem.AddRange(
            new RuleItem
            {
                RuleId = Guid.NewGuid(),
                OwnerId = ownerId1,
                Event = RuleEvent.TaskCreated,
                Condition = "{}",
                Action = "[]"
            },
            new RuleItem
            {
                RuleId = Guid.NewGuid(),
                OwnerId = ownerId2,
                Event = RuleEvent.TaskDeleted,
                Condition = "{}",
                Action = "[]"
            }
        );

        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetRulesQuery
        (
            OwnerId: ownerId1,
            RuleEvent: RuleEvent.TaskCreated
        ));

        Assert.Single(result.Rules);
    }
}
