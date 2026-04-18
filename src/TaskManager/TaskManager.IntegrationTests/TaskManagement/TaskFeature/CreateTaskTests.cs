using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class CreateTaskTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public CreateTaskTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Create_Task()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var command = new CreateTaskCommand("proj", "Test task", "desc", "New", "Low", "", "");

        var taskId = await mediator.Send(command);

        var task = await context.TaskItems.FindAsync(Guid.Parse(taskId));

        Assert.NotNull(task);
        Assert.Equal(user.UserId, task.OwnerId);
        Assert.Equal("Test task", task.Title);
    }
}
