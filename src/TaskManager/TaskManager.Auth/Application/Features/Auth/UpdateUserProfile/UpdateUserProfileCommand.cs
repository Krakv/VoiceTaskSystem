using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.UpdateUserProfile;

public record UpdateUserProfileCommand(
    string Name,
    string? Email
) : IRequest;
