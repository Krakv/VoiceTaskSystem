using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class PatchTaskTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public PatchTaskTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Patch_Only_Provided_Fields()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = (FakeCurrentUser)_provider.GetRequiredService<ICurrentUser>();

        var task = new TaskItemBuilder(user.UserId)
            .SetTitle("old")
            .SetDescription("desc")
            .Build();

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        await mediator.Send(new UpdateTaskPatchCommand(user.UserId, task.TaskId, Title: "patched"));

        var updated = await context.TaskItems.FindAsync(task.TaskId);

        Assert.NotNull(updated);
        Assert.Equal("patched", updated.Title);
        Assert.Equal("desc", updated.Description);
    }
}
