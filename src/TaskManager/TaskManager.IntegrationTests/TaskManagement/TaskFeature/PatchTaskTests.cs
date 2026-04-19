using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class UpdateTaskTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public UpdateTaskTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Update_Task()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var task = new TaskItemBuilder(user.UserId)
            .SetTitle("old")
            .Build();

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        await mediator.Send(new UpdateTaskCommand(user.UserId, task.TaskId, "", "new", "", TaskItemStatus.New, TaskItemPriority.Low, null, null));

        var updated = await context.TaskItems.FindAsync(task.TaskId);

        Assert.NotNull(updated);
        Assert.Equal("new", updated.Title);
    }
}
