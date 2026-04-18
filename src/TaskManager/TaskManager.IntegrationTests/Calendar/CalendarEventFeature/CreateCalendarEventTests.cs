using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;

namespace TaskManager.IntegrationTests.Calendar.CalendarEventFeature;

public class CreateCalendarEventTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public CreateCalendarEventTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Create_Event_And_Publish_Event()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = Guid.NewGuid();

        var command = new CreateCalendarEventCommand(
            ownerId.ToString(),
            "title",
            "2024-01-01T10:00:00Z",
            "2024-01-01T11:00:00Z",
            "loc",
            null,
            null
        );

        var eventId = await mediator.Send(command);

        var entity = await context.CalendarEvent.FindAsync(eventId);

        Assert.NotNull(entity);
        Assert.Equal("title", entity.Title);
    }
}
