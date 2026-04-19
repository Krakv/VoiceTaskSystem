using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class PatchTaskTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Patch_Only_Provided_Fields()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();

        var task = new TaskItemBuilder(userId)
            .SetTitle("old")
            .SetDescription("desc")
            .Build();

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        await mediator.Send(new UpdateTaskPatchCommand(userId, task.TaskId, Title: "patched"));

        var updated = await context.TaskItems.FindAsync(task.TaskId);

        Assert.NotNull(updated);
        Assert.Equal("patched", updated.Title);
        Assert.Equal("desc", updated.Description);
    }
}
