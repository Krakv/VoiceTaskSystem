using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.ChangeMyPassword;

public record ChangeMyPasswordCommand(
    Guid OwnerId,
    string CurrentPassword,
    string NewPassword
) : IRequest<bool>;
