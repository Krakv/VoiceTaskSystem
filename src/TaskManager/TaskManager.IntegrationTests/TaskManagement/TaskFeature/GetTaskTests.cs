using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class GetTasksTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public GetTasksTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Filter_Tasks_By_Title()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        context.TaskItems.AddRange(
            new TaskItemBuilder(user.UserId).SetTitle("apple").Build(),
            new TaskItemBuilder(user.UserId).SetTitle("banana").Build()
        );

        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetTasksQuery(user.UserId, "apple", null, null, null, null));

        Assert.Single(result.Tasks);
    }
}
