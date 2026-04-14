namespace TaskManager.Shared.Interfaces;

public interface INotificationAccessRequest
{
    string NotificationId { get; } 
    string OwnerId { get; }
}
