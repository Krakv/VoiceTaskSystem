using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class GetTaskTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public GetTaskTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Return_Task()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var task = new TaskItemBuilder(user.UserId)
            .SetTitle("task")
            .Build();

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetTaskQuery(user.UserId, task.TaskId));

        Assert.Equal(task.Title, result.Title);
    }
}
