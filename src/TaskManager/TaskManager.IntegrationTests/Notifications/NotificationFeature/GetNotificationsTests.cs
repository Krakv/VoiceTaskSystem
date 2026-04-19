using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotifications;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.IntegrationTests.Notifications.NotificationFeature;

public class GetNotificationsTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public GetNotificationsTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Get_All_Notifications_For_Owner()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var ownerId = Guid.NewGuid();

        await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "First",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1)
        ));

        await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Second",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(2)
        ));

        var response = await mediator.Send(new GetNotificationsQuery(ownerId));

        Assert.NotNull(response);
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task Should_Return_Empty_List_For_Unknown_Owner()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new GetNotificationsQuery(Guid.NewGuid()));

        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task Should_Not_Return_Notifications_Of_Other_Owners()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var ownerA = Guid.NewGuid();
        var ownerB = Guid.NewGuid();

        await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerA,
            ServiceId: NotificationServiceType.Email,
            Description: "Owner A notification",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1)
        ));

        var response = await mediator.Send(new GetNotificationsQuery(ownerB));

        Assert.DoesNotContain(response, n => n.Description == "Owner A notification");
    }

    [Fact]
    public async Task Should_Return_Pending_Notifications_First()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = Guid.NewGuid();

        var _ = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Pending",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(3)
        ));

        var sentId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Sent",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1)
        ));

        var sentEntity = await context.NotificationItem.FindAsync(sentId);
        sentEntity!.Status = NotificationStatus.Sent;
        await context.SaveChangesAsync();

        var response = await mediator.Send(new GetNotificationsQuery(ownerId));

        Assert.Equal("Pending", response[0].Description);
    }
}
