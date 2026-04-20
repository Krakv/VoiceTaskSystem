using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Auth.Application.Features.Auth.GenerateTelegramLinkToken;

public class GenerateTelegramLinkTokenHandler(
    UserManager<User> userManager)
    : IRequestHandler<GenerateTelegramLinkTokenCommand, string>
{
    public async Task<string> Handle(
        GenerateTelegramLinkTokenCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.OwnerId, cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Пользователь не найден");

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));

        user.TelegramToken = token;
        user.TelegramTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);

        await userManager.UpdateAsync(user);

        return token;
    }
}
