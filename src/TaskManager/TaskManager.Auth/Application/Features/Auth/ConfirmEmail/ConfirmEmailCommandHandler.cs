using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Auth.Application.Features.Auth.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    UserManager<User> userManager
) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.OwnerId, cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Пользователь не найден");

        if (user.EmailConfirmed)
            throw new ValidationAppException("CONFLICT", "Почта уже подтверждена");

        var decodedToken = Uri.UnescapeDataString(request.Token);

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new ValidationAppException("INVALID_PARAMS", errors);
        }
    }
}
