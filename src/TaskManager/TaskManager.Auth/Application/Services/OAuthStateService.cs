using System.Text;
using TaskManager.Auth.Application.Interfaces;

namespace TaskManager.Auth.Application.Services;

public class OAuthStateService : IStateService
{
    public string Generate(Guid userId)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(userId.ToString())
        );
    }

    public Guid Parse(string state)
    {
        var userId = Encoding.UTF8.GetString(Convert.FromBase64String(state));
        return Guid.Parse(userId);
    }
}
