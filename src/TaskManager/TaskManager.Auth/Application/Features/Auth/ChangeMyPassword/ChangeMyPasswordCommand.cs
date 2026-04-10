using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.ChangeMyPassword;

public record ChangeMyPasswordCommand(
    string CurrentPassword,
    string NewPassword
) : IRequest<bool>;
