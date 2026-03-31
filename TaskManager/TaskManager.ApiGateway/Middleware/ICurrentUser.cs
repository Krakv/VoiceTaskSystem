namespace TaskManager.ApiGateway.Middleware;

public interface ICurrentUser
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}

