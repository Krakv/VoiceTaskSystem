using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.ToggleRule;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class ToggleRuleTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public ToggleRuleTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Toggle_Rule()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var rule = new RuleItem
        {
            RuleId = Guid.NewGuid(),
            OwnerId = user.UserId,
            IsActive = true
        };

        context.RuleItem.Add(rule);
        await context.SaveChangesAsync();

        await mediator.Send(new ToggleRuleCommand(rule.RuleId.ToString()));

        var updated = await context.RuleItem.FindAsync(rule.RuleId);

        Assert.False(updated.IsActive);
    }
}
