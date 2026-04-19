using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvent;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;

namespace TaskManager.IntegrationTests.Calendar.CalendarEventFeature;

public class GetCalendarEventTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public GetCalendarEventTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Return_Event()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = Guid.NewGuid();

        var entity = new Shared.Domain.Entities.CalendarEvent
        {
            EventId = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = "test"
        };

        context.CalendarEvent.Add(entity);
        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetCalendarEventQuery(
            ownerId,
            entity.EventId
        ));

        Assert.Equal("test", result.Title);
    }
}
