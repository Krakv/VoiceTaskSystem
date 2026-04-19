using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.DeleteUser;

public sealed record DeleteUserCommand(Guid OwnerId) : IRequest<bool>;
