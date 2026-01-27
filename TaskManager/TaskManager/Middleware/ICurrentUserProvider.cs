namespace TaskManager.Middleware;

public interface ICurrentUserProvider
{
    ICurrentUser GetCurrentUser();
}

