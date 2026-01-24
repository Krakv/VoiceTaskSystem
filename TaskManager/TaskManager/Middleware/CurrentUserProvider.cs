namespace TaskManager.Middleware;

public sealed class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public ICurrentUser GetCurrentUser()
    {
        return new HttpCurrentUser(_httpContextAccessor);
    }
}

