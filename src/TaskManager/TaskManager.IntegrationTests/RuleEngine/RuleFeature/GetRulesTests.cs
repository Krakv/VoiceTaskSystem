using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class GetRulesTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public GetRulesTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Filter_By_Event()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        context.RuleItem.AddRange(
            new RuleItem
            {
                RuleId = Guid.NewGuid(),
                OwnerId = user.UserId,
                Event = RuleEvent.TaskCreated,
                Condition = "{}",
                Action = "[]"
            },
            new RuleItem
            {
                RuleId = Guid.NewGuid(),
                OwnerId = user.UserId,
                Event = RuleEvent.TaskDeleted,
                Condition = "{}",
                Action = "[]"
            }
        );

        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetRulesQuery
        {
            RuleEvent = "TaskCreated"
        });

        Assert.Single(result.Rules);
    }
}
