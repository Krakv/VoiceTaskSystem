using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;

namespace TaskManager.IntegrationTests.Calendar.CalendarEventFeature;

public class GetCalendarEventsTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Return_Only_User_Events()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId1 = await fixture.CreateUserAsync();
        var ownerId2 = await fixture.CreateUserAsync("test2");

        context.CalendarEvent.AddRange(
            new Shared.Domain.Entities.CalendarEvent
            {
                EventId = Guid.NewGuid(),
                Title = "Event 1",
                OwnerId = ownerId1
            },
            new Shared.Domain.Entities.CalendarEvent
            {
                EventId = Guid.NewGuid(),
                Title = "Event 2",
                OwnerId = ownerId2
            }
        );

        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetCalendarEventsQuery(ownerId1));

        Assert.Single(result);
    }
}
