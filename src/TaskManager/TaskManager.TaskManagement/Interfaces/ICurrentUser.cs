namespace TaskManager.TaskManagement.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}

