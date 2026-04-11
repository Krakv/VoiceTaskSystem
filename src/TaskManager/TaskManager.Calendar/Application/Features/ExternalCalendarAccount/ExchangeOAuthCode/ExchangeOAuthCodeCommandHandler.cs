using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Auth.Application.Interfaces;
using TaskManager.Auth.Infrastructure;
using TaskManager.Calendar.Infrastructure.Interfaces;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.ExchangeOAuthCode;

public sealed class ExchangeOAuthCodeCommandHandler(YandexOAuthClient oAuthClient, AppDbContext context, ICalDavClient calDavClient, IStateService stateService) : IRequestHandler<ExchangeOAuthCodeCommand>
{
    private readonly YandexOAuthClient _oAuthClient = oAuthClient;
    private readonly AppDbContext _context = context;
    private readonly ICalDavClient _calDavClient = calDavClient;
    private readonly IStateService _stateService = stateService;
    public async Task Handle(ExchangeOAuthCodeCommand request, CancellationToken cancellationToken)
    {
        var token = await _oAuthClient.ExchangeCodeAsync(request.Code);
        var userId = _stateService.Parse(request.State);

        var calDavEmail = await _calDavClient.GetUserEmailAsync(token.access_token, cancellationToken);
        var baseUrl = $"https://caldav.yandex.ru/calendars/{calDavEmail}/events-default/";

        var existing = await _context.ExternalCalendarAccount
            .FirstOrDefaultAsync(x => 
            x.OwnerId == userId &&
            x.BaseUrl == baseUrl, 
            cancellationToken);

        if (existing is null)
        {
            var account = new Shared.Domain.Entities.ExternalCalendarAccount
            {
                OwnerId = userId,
                BaseUrl = baseUrl,
                AccessToken = token.access_token,
                RefreshToken = token.refresh_token ?? throw new ValidationAppException("INVALID_PARAMS","Нет refresh токена"),
                ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(token.expires_in)
            };

            await _context.ExternalCalendarAccount.AddAsync(account, cancellationToken);
        }
        else
        {
            existing.AccessToken = token.access_token;

            if (!string.IsNullOrWhiteSpace(token.refresh_token))
            {
                existing.RefreshToken = token.refresh_token;
            }

            existing.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(token.expires_in);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // fallback: кто-то уже создал
            var ex = await _context.ExternalCalendarAccount
                .FirstAsync(x =>
                    x.OwnerId == userId &&
                    x.BaseUrl == baseUrl,
                    cancellationToken);

            ex.AccessToken = token.access_token;

            if (!string.IsNullOrWhiteSpace(token.refresh_token))
            {
                ex.RefreshToken = token.refresh_token;
            }

            ex.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(token.expires_in);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
