using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Auth.Application.Features.Auth.LinkTelegram;

public class LinkTelegramCommandHandler(UserManager<User> userManager) : IRequestHandler<LinkTelegramCommand>
{
    public async Task Handle(LinkTelegramCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.TelegramToken == request.Token, cancellationToken)
            ?? throw new ValidationAppException("INVALID_TOKEN", "Неверный токен");
        
        if (user.TelegramTokenExpiresAt < DateTimeOffset.UtcNow)
            throw new ValidationAppException("TOKEN_EXPIRED", "Токен истёк");

        user.TelegramChatId = request.ChatId;

        user.TelegramToken = null;
        user.TelegramTokenExpiresAt = null;

        await userManager.UpdateAsync(user);
    }
}
