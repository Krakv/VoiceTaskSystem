namespace TaskManager.Shared.Interfaces;

public interface INotificationAccessRequest
{
    Guid NotificationId { get; } 
    Guid OwnerId { get; }
}
