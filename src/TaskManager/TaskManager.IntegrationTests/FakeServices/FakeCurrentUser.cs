using TaskManager.Shared.Interfaces;

namespace TaskManager.IntegrationTests.FakeServices;

public class FakeCurrentUser : ICurrentUser
{
    public Guid UserId { get; set; } = Guid.NewGuid();

    public bool IsAuthenticated => true;
}
