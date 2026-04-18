using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.DeleteRule;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.RuleEngine.RuleFeature;

public class DeleteRuleTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public DeleteRuleTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Delete_Rule()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var rule = new RuleItem
        {
            RuleId = Guid.NewGuid(),
            OwnerId = user.UserId
        };

        context.RuleItem.Add(rule);
        await context.SaveChangesAsync();

        await mediator.Send(new DeleteRuleCommand(rule.RuleId.ToString()));

        var deleted = await context.RuleItem.FindAsync(rule.RuleId);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task Should_Throw_When_Not_Owner()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var rule = new RuleItem
        {
            RuleId = Guid.NewGuid(),
            OwnerId = Guid.NewGuid()
        };

        context.RuleItem.Add(rule);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new DeleteRuleCommand(rule.RuleId.ToString())));
    }
}
