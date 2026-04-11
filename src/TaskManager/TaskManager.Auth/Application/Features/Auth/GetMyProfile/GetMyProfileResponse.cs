namespace TaskManager.Auth.Application.Features.Auth.GetMyProfile;

public sealed record GetMyProfileResponse(Guid Id, string Name, string? Email);
