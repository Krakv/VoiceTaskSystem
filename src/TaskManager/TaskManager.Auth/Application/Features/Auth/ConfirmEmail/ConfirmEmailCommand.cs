using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.ConfirmEmail;

public record ConfirmEmailCommand(string Token) : IRequest;
