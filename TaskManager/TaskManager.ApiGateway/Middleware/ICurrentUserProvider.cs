namespace TaskManager.ApiGateway.Middleware;

public interface ICurrentUserProvider
{
    ICurrentUser GetCurrentUser();
}

