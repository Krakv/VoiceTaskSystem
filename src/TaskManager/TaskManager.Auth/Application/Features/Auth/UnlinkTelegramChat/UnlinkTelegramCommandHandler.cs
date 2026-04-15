using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Auth.Application.Features.Auth.UnlinkTelegramChat;

public class UnlinkTelegramCommandHandler(
    UserManager<User> userManager,
    ICurrentUser currentUser,
    ILogger<UnlinkTelegramCommandHandler> logger)
    : IRequestHandler<UnlinkTelegramCommand>
{
    public async Task Handle(UnlinkTelegramCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == currentUser.UserId, cancellationToken);

        if (user is null)
            throw new ValidationAppException("NOT_FOUND", "Пользователь не найден");

        if (user.TelegramChatId is null)
            throw new ValidationAppException("CONFLICT", "Telegram не привязан");

        user.TelegramChatId = null;

        user.TelegramToken = null;
        user.TelegramTokenExpiresAt = null;

        await userManager.UpdateAsync(user);

        logger.LogInformation("Telegram unlinked for user {UserId}", user.Id);
    }
}
