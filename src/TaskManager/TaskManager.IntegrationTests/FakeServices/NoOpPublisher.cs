using MediatR;

namespace TaskManager.IntegrationTests.FakeServices;

public class NoOpPublisher : INotificationPublisher
{
    public Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
