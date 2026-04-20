using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.IntegrationTests.Notifications.NotificationFeature;

public class CreateNotificationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Create_Notification()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = await fixture.CreateUserAsync();
        var scheduledAt = DateTimeOffset.UtcNow.AddHours(1);

        var command = new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,    
            ServiceId: NotificationServiceType.Email,
            Description: "Test notification",
            ScheduledAt: scheduledAt
        );

        var notificationId = await mediator.Send(command);

        var entity = await context.NotificationItem.FindAsync(notificationId);

        Assert.NotNull(entity);
        Assert.Equal(notificationId, entity.NotificationId);
        Assert.Equal(ownerId, entity.OwnerId);
        Assert.Null(entity.TaskId);
        Assert.Equal("Test notification", entity.Description);
    }

    [Fact]
    public async Task Should_Create_Notification_Without_TaskId()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var ownerId = await fixture.CreateUserAsync();

        var command = new CreateNotificationCommand(
            TaskId: null,
            OwnerId: ownerId,
            ServiceId: NotificationServiceType.Email,
            Description: "Notification without task",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(1)
        );

        var notificationId = await mediator.Send(command);

        var entity = await context.NotificationItem.FindAsync(notificationId);

        Assert.NotNull(entity);
        Assert.Null(entity.TaskId);
        Assert.Equal(ownerId, entity.OwnerId);
    }

    [Fact]
    public async Task Should_Return_New_Guid_On_Create()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var command = new CreateNotificationCommand(
            TaskId: null,
            OwnerId: await fixture.CreateUserAsync(),
            ServiceId: NotificationServiceType.Email,
            Description: "Another notification",
            ScheduledAt: DateTimeOffset.UtcNow.AddHours(2)
        );

        var notificationId = await mediator.Send(command);

        Assert.NotEqual(Guid.Empty, notificationId);
    }
}
