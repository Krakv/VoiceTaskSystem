using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Calendar.Application.Features.CalendarEvent.DeleteCalendarEvent;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.IntegrationTests.Calendar.CalendarEventFeature;

public class DeleteCalendarEventTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Delete_Event_And_Publish_Event()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = await fixture.CreateUserAsync();

        var entity = new CalendarEvent
        {
            EventId = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = "test",
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddHours(1)
        };

        context.CalendarEvent.Add(entity);
        await context.SaveChangesAsync();

        await mediator.Send(new DeleteCalendarEventCommand(
            ownerId,
            entity.EventId
        ));

        var deleted = await context.CalendarEvent.FindAsync(entity.EventId);

        Assert.Null(deleted);
    }
}
