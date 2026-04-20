using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.UpdateUserProfile;

public record UpdateUserProfileCommand(
    Guid OwnerId,
    string? Name,
    string? Email
) : IRequest;
