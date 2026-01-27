using System.Security.Claims;

namespace TaskManager.Middleware;

public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public Guid UserId =>
        Guid.Parse(_accessor.HttpContext!
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

    public bool IsAuthenticated => 
        _accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
}

