using MediatR;

namespace TaskManager.IntegrationTests.FakeServices;

public class TestPublisher : IPublisher
{
    public List<INotification> Events { get; } = new();

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        Events.Add(notification);
        return Task.CompletedTask;
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        Events.Add((INotification)notification);
        return Task.CompletedTask;
    }
}
