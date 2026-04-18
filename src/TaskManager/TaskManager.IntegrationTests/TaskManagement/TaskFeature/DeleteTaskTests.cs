using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;


public class DeleteTaskTest : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public DeleteTaskTest(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Delete_Task()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var task = new TaskItemBuilder(user.UserId)
            .SetTitle("to delete")
            .Build();

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        await mediator.Send(new DeleteTaskCommand(task.TaskId.ToString()));

        var deleted = await context.TaskItems.FindAsync(task.TaskId);

        Assert.Null(deleted);
    }
}