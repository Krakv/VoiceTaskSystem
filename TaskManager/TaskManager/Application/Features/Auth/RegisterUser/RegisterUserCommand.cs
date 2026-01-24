namespace TaskManager.Application.Features.Auth.RegisterUser;

using MediatR;

public record RegisterUserCommand(
    string UserName,
    string Email,
    string Password,
    string Name
) : IRequest<Guid>;

