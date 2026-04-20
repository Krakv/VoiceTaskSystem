using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class UpdateTaskTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Update_Task()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();

        var task = new TaskItemBuilder(userId)
            .SetTitle("old")
            .Build();

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        await mediator.Send(new UpdateTaskCommand(userId, task.TaskId, "", "new", "", TaskItemStatus.New, TaskItemPriority.Low, null, null));

        var updated = await context.TaskItems.FindAsync(task.TaskId);

        Assert.NotNull(updated);
        Assert.Equal("new", updated.Title);
    }
}
