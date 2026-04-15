using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.SendEmailVerification;

public record SendEmailVerificationCommand() : IRequest;
