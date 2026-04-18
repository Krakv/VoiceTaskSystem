using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;

namespace TaskManager.IntegrationTests.Calendar.CalendarEventFeature;

public class UpdateCalendarEventTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public UpdateCalendarEventTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Update_Event_And_Publish_Event()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = Guid.NewGuid();

        var entity = new Shared.Domain.Entities.CalendarEvent
        {
            EventId = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = "old"
        };

        context.CalendarEvent.Add(entity);
        await context.SaveChangesAsync();

        await mediator.Send(new UpdateCalendarEventCommand(
            ownerId.ToString(),
            entity.EventId.ToString(),
            "new",
            "2024-01-01T10:00:00Z",
            "2024-01-01T11:00:00Z",
            "loc",
            null,
            null
        ));

        var updated = await context.CalendarEvent.FindAsync(entity.EventId);

        Assert.Equal("new", updated.Title);
    }
}
