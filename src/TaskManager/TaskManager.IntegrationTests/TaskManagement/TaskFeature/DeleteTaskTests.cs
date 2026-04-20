using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;


public class DeleteTaskTest(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Delete_Task()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();
        var task = new TaskItemBuilder(userId)
            .SetTitle("to delete")
            .Build();

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        await mediator.Send(new DeleteTaskCommand(userId, task.TaskId));

        var deleted = await context.TaskItems.FindAsync(task.TaskId);

        Assert.Null(deleted);
    }
}