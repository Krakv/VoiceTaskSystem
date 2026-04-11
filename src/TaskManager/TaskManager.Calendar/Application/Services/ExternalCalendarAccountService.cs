using Microsoft.EntityFrameworkCore;
using TaskManager.Auth.Infrastructure;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Calendar.Application.Services;

public sealed class ExternalCalendarAccountService(AppDbContext db, YandexOAuthClient oauth)
{
    private readonly AppDbContext _db = db;
    private readonly YandexOAuthClient _oauth = oauth;

    public async Task<ExternalCalendarAccount> GetValidAccount(Guid id)
    {
        var account = await _db.ExternalCalendarAccount
            .FirstOrDefaultAsync(x => x.ExternalCalendarAccountId == id);

        if (account == null)
            throw new ValidationAppException("NOT_FOUND", "Аккаунт не найден");

        if (account.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
            return account;

        var token = await _oauth.RefreshTokenAsync(account.RefreshToken);

        account.AccessToken = token.access_token;

        if (!string.IsNullOrWhiteSpace(token.refresh_token))
            account.RefreshToken = token.refresh_token;

        account.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(token.expires_in);

        await _db.SaveChangesAsync();

        return account;
    }
}
