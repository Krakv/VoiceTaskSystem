using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.ConfirmEmail;

public record ConfirmEmailCommand(Guid OwnerId, string Token) : IRequest;
