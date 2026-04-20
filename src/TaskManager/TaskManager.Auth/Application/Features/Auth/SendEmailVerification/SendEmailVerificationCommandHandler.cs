using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskManager.Auth.Config;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Auth.Application.Features.Auth.SendEmailVerification;

public sealed class SendEmailVerificationCommandHandler(
    UserManager<User> userManager,
    IEmailService emailService,
    IOptions<FrontendOptions> options
) : IRequestHandler<SendEmailVerificationCommand>
{
    public async Task Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var userId = request.OwnerId;

        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.OwnerId, cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Пользователь не найден");

        if (user.IsDeleted)
            throw new ValidationAppException("CONFLICT", "Пользователь удален");

        if (user.EmailConfirmed)
            throw new ValidationAppException("CONFLICT", "Почта уже подтверждена");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = Uri.EscapeDataString(token);

        var frontendUrl = options.Value.Url;
        var confirmUrl = $"{frontendUrl}/email-confirm?token={encodedToken}";

        var body = $@"
            <h2>Подтверждение email</h2>
            <p>Нажмите на ссылку для подтверждения:</p>
            <a href='{confirmUrl}'>Подтвердить email</a>
            <p>Если это сообщение попало к Вам по ошибке, проигнорируйте его.</p>
        ";

        await emailService.SendAsync(user.Email!, "Подтверждение email", body);
    }
}
