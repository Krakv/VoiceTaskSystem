using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Auth.Application.Features.Auth.ChangeMyPassword;

public sealed class ChangeMyPasswordCommandHandler(
    UserManager<User> userManager,
    ILogger<ChangeMyPasswordCommandHandler> logger)
    : IRequestHandler<ChangeMyPasswordCommand, bool>
{
    public async Task<bool> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.OwnerId, cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Пользователь не найден");

        if (user.IsDeleted)
            throw new ValidationAppException(
                "CONFLICT",
                "Пользователь удален"
            );

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword
        );

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(e => "Password")
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(' ', g.Select(x => x.Description).ToArray())
                );

            throw new ValidationAppException("INVALID_PARAMS", errors);
        }

        logger.LogInformation("Password changed for user {UserId}", user.Id);

        return true;
    }
}
