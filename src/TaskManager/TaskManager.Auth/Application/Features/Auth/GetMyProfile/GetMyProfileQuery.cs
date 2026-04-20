using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.GetMyProfile;

public record GetMyProfileQuery(Guid OwnerId) : IRequest<GetMyProfileResponse>;
