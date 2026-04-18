using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.NotificationFeature;

public class UpdateNotificationTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider;

    public UpdateNotificationTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Should_Update_Notification()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var ownerId = Guid.NewGuid().ToString();

        var notificationId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Original description",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1).ToString("O")
        ));

        var newScheduledAt = DateTimeOffset.UtcNow.AddHours(5);

        await mediator.Send(new UpdateNotificationCommand(
            OwnerId: ownerId,
            NotificationId: notificationId.ToString(),
            Description: "Updated description",
            ScheduledAt: newScheduledAt.ToString("O")
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
            OwnerId: user.UserId.ToString(),
            NotificationId: Guid.NewGuid().ToString(),
            Description: "Won't be applied",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1).ToString("O")
        );

        var exception = await Record.ExceptionAsync(() => mediator.Send(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task Should_Not_Update_Other_Notifications()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();
        var ownerId = Guid.NewGuid().ToString();

        var firstId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Should stay unchanged",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1).ToString("O")
        ));

        var secondId = await mediator.Send(new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Will be updated",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(2).ToString("O")
        ));

        await mediator.Send(new UpdateNotificationCommand(
            OwnerId: ownerId,
            NotificationId: secondId.ToString(),
            Description: "Updated",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(3).ToString("O")
        ));

        var first = await context.NotificationItem.FindAsync(firstId);
        Assert.Equal("Should stay unchanged", first!.Description);
    }
}
