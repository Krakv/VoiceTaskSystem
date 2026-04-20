using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Builders;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

namespace TaskManager.IntegrationTests.TaskManagement.TaskFeature;

public class GetTasksTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Filter_Tasks_By_Title()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var userId = await fixture.CreateUserAsync();

        context.TaskItems.AddRange(
            new TaskItemBuilder(userId).SetTitle("apple").Build(),
            new TaskItemBuilder(userId).SetTitle("banana").Build()
        );

        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetTasksQuery(userId, "apple", null, null, null, null));

        Assert.Single(result.Tasks);
    }
}
