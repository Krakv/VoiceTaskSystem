using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class GetTaskTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Return_Task()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();

        var task = new TaskItemBuilder(userId)
            .SetTitle("task")
            .Build();

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetTaskQuery(userId, task.TaskId));

        Assert.Equal(task.Title, result.Title);
    }
}
