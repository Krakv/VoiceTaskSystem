namespace TaskManager.Middleware;

public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public Guid UserId =>
        Guid.Parse(_accessor.HttpContext!.User.FindFirst("sub")!.Value);

    public bool IsAuthenticated => 
        _accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
}

