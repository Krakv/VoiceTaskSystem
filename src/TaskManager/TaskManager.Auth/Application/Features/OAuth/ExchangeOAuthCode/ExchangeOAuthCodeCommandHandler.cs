using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Auth.Application.Interfaces;
using TaskManager.Auth.Infrastructure;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.Auth.Application.Features.OAuth.ExchangeOAuthCode;

public sealed class ExchangeOAuthCodeCommandHandler(YandexOAuthClient oAuthClient, AppDbContext context, IStateService stateService) : IRequestHandler<ExchangeOAuthCodeCommand>
{
    private readonly YandexOAuthClient _oAuthClient = oAuthClient;
    private readonly AppDbContext _context = context;
    private readonly IStateService _stateService = stateService;
    public async Task Handle(ExchangeOAuthCodeCommand request, CancellationToken cancellationToken)
    {
        var token = await _oAuthClient.ExchangeCodeAsync(request.Code);

        var userId = _stateService.Parse(request.State);

        var baseUrl = "https://caldav.yandex.ru/";

        var existing = await _context.ExternalCalendarAccount
            .FirstOrDefaultAsync(x => 
            x.OwnerId == userId &&
            x.BaseUrl == baseUrl, 
            cancellationToken);

        if (existing is null)
        {
            var account = new ExternalCalendarAccount
            {
                OwnerId = userId,
                BaseUrl = baseUrl,
                AccessToken = token.access_token,
                RefreshToken = token.refresh_token ?? throw new Exception("No refresh token"),
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
