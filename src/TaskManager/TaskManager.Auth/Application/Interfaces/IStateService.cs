namespace TaskManager.Auth.Application.Interfaces;

public interface IStateService
{
    string Generate(Guid userId);

    Guid Parse(string state);
}
