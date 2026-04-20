using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;

namespace TaskManager.IntegrationTests.Calendar.CalendarEventFeature;

public class UpdateCalendarEventTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Update_Event_And_Publish_Event()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = await fixture.CreateUserAsync();

        var entity = new Shared.Domain.Entities.CalendarEvent
        {
            EventId = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = "old"
        };

        context.CalendarEvent.Add(entity);
        await context.SaveChangesAsync();

        await mediator.Send(new UpdateCalendarEventCommand(
            ownerId,
            entity.EventId,
            "new",
            DateTimeOffset.Parse("2024-01-01T10:00:00Z", CultureInfo.InvariantCulture),
            DateTimeOffset.Parse("2024-01-01T11:00:00Z", CultureInfo.InvariantCulture),
            "loc",
            null,
            null
        ));

        var updated = await context.CalendarEvent.FindAsync(entity.EventId);

        Assert.NotNull(updated);
        Assert.Equal("new", updated.Title);
    }
}
