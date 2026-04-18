using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Features.NotificationFeature.DeleteNotification;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.Notifications.NotificationFeature;

public class DeleteNotificationTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public DeleteNotificationTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Delete_Notification()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var user = _provider.GetRequiredService<ICurrentUser>();

        var notificationId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: Guid.NewGuid().ToString(),
            ServiceId: NotificationServiceType.Email,
            Description: "To be deleted",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1).ToString("O")
        ));

        await mediator.Send(new DeleteNotificationCommand(user.UserId.ToString(), notificationId.ToString()));

        var entity = await context.NotificationItem.FindAsync(notificationId);

        Assert.Null(entity);
    }

    [Fact]
    public async Task Should_Throw_When_Notification_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var command = new DeleteNotificationCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        await Assert.ThrowsAsync<ValidationAppException>(() => mediator.Send(command));
    }

    [Fact]
    public async Task Should_Not_Delete_Other_Notifications()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = Guid.NewGuid().ToString();

        var firstId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "First",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1).ToString("O")
        ));

        var secondId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Second",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(2).ToString("O")
        ));

        await mediator.Send(new DeleteNotificationCommand(ownerId, firstId.ToString()));

        var second = await context.NotificationItem.FindAsync(secondId);
        Assert.NotNull(second);
    }
}
