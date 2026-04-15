using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Auth.Application.Features.Auth.GetMyProfile;

public sealed class GetMyProfileQueryHandler(UserManager<User> userManager, ICurrentUser currentUser) : IRequestHandler<GetMyProfileQuery, GetMyProfileResponse>
{
    public async Task<GetMyProfileResponse> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var user = await userManager.Users
            .Include(x => x.ExternalCalendarAccounts)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
            throw new ValidationAppException("NOT_FOUND", "Пользователь не найден");

        if (user.IsDeleted)
            throw new ValidationAppException("CONFLICT", "Пользователь удален");

        return new GetMyProfileResponse(user.Id, user.Name, user.Email, user.EmailConfirmed, user.ExternalCalendarAccounts.Select(x => x.ExternalCalendarAccountId).ToList(), user.TelegramChatId);
    }
}
