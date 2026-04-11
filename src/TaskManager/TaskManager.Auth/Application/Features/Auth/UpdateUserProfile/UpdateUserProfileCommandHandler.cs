using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Auth.Application.Features.Auth.UpdateUserProfile;

public sealed class UpdateMyProfileCommandHandler(UserManager<User> userManager, ICurrentUser currentUser, ILogger<UpdateMyProfileCommandHandler> logger)
    : IRequestHandler<UpdateUserProfileCommand>
{
    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
            throw new ValidationAppException("NOT_FOUND", "Пользователь не найден");

        if (user.IsDeleted)
            throw new ValidationAppException("CONFLICT", "Пользователь уже удален");

        var errors = new Dictionary<string, string>();

        user.Name = request.Name;

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            var emailResult = await userManager.SetEmailAsync(user, request.Email);

            if (!emailResult.Succeeded)
            {
                errors["Email"] = string.Join(' ', 
                    emailResult.Errors
                    .Select(e => e.Description)
                    .ToArray());
            }
        }

        if (errors.Count > 0)
        {
            throw new ValidationAppException("INVALID_PARAMS", errors);
        }

        await userManager.UpdateAsync(user);

        logger.LogInformation("Profile updated for user {UserId}", userId);
    }
}
