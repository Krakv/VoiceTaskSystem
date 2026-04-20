using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.Notifications.NotificationFeature;

public class GetNotificationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Get_Notification_By_Id()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var ownerId = await fixture.CreateUserAsync();
        var serviceId = NotificationServiceType.Email;

        var notificationId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: serviceId,
            Description: "Get me",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1)
        ));

        var response = await mediator.Send(new GetNotificationQuery(ownerId, notificationId));

        Assert.NotNull(response);
        Assert.Equal(notificationId, response.NotificationId);
        Assert.Equal(serviceId, response.ServiceId);
        Assert.Equal("Get me", response.Description);
    }

    [Fact]
    public async Task Should_Return_Null_When_Notification_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = _provider.GetRequiredService<ICurrentUser>();

        var exception = await Record.ExceptionAsync(() => mediator.Send(new GetNotificationQuery(user.UserId, Guid.NewGuid())));
        Assert.NotNull(exception);
    }
}
