using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class CreateTaskTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Create_Task()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();

        var command = new CreateTaskCommand(userId, "proj", "Test task", "desc", TaskItemStatus.New, TaskItemPriority.Low, null, null);

        var taskId = await mediator.Send(command);

        var task = await context.TaskItems.FindAsync(taskId);

        Assert.NotNull(task);
        Assert.Equal(userId, task.OwnerId);
        Assert.Equal("Test task", task.Title);
    }
}
