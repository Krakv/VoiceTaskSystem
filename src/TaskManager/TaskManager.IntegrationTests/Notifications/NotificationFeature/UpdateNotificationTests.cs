using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.Notifications.NotificationFeature;

public class UpdateNotificationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Update_Notification()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var ownerId = await fixture.CreateUserAsync();

        var notificationId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Original description",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1)
        ));

        var newScheduledAt = DateTimeOffset.UtcNow.AddHours(5);

        await mediator.Send(new UpdateNotificationCommand(
            OwnerId: ownerId,
            NotificationId: notificationId,
            Description: "Updated description",
            ScheduledAt: newScheduledAt
        ));

        var entity = await context.NotificationItem.FindAsync(notificationId);

        Assert.NotNull(entity);
        Assert.Equal("Updated description", entity.Description);
        Assert.Equal(newScheduledAt.ToUnixTimeSeconds(), entity.ScheduledAt.ToUnixTimeSeconds());
    }

    [Fact]
    public async Task Should_Not_Throw_When_Notification_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = _provider.GetRequiredService<ICurrentUser>();

        var command = new UpdateNotificationCommand(
            OwnerId: user.UserId,
            NotificationId: Guid.NewGuid(),
            Description: "Won't be applied",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1)
        );

        var exception = await Record.ExceptionAsync(() => mediator.Send(command));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task Should_Not_Update_Other_Notifications()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var ownerId = await fixture.CreateUserAsync();

        var firstId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Should stay unchanged",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1)
        ));

        var secondId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Will be updated",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(2)
        ));

        await mediator.Send(new UpdateNotificationCommand(
            OwnerId: ownerId,
            NotificationId: secondId,
            Description: "Updated",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(3)
        ));

        var first = await context.NotificationItem.FindAsync(firstId);
        Assert.Equal("Should stay unchanged", first!.Description);
    }
}
