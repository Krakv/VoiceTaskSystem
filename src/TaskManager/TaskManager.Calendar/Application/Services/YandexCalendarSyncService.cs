using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Calendar.Application.Interfaces;
using TaskManager.Calendar.Infrastructure.Interfaces;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.Calendar.Application.Services;

public sealed class YandexCalendarSyncService(AppDbContext dbContext, ICalDavClient client, ILogger<YandexCalendarSyncService> logger, ExternalCalendarAccountService accountService, ICalendarIcsGenerator icsGenerator) : ICalendarSyncService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ICalDavClient _client = client;
    private readonly ILogger<YandexCalendarSyncService> _logger = logger;
    private readonly ExternalCalendarAccountService _accountService = accountService;
    private readonly ICalendarIcsGenerator _icsGenerator = icsGenerator;

    public async Task SyncAllAsync(Guid externalAccountId, CancellationToken cancellationToken)
    {
        var account = await _accountService.GetValidAccount(externalAccountId);

        var events = await _dbContext.CalendarEvent
            .Where(x => x.ExternalAccountId == externalAccountId)
            .ToListAsync(cancellationToken);

        foreach (var e in events)
        {
            var ics = _icsGenerator.Generate(e);

            var eventUrl = _client.BuildEventUrl(account.BaseUrl, e.EventId.ToString());
            await _client.UpdateEventAsync(
                    eventUrl,
                    ics,
                    account.AccessToken,
                    cancellationToken);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task SyncDeleteEventAsync(Guid calendarEventId, Guid? externalAccountId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(externalAccountId.ToString(), out Guid accountId)) return;

        var account = await _accountService.GetValidAccount(accountId);

        var eventUrl = _client.BuildEventUrl(account.BaseUrl, calendarEventId.ToString());

        await _client.DeleteEventAsync(
                eventUrl,
                account.AccessToken,
                cancellationToken);
    }

    public async Task SyncEventAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(calendarEvent.ExternalAccountId.ToString(), out Guid accountId)) return;

        var account = await _accountService.GetValidAccount(accountId);

        var ics = _icsGenerator.Generate(calendarEvent);

        var eventUrl = _client.BuildEventUrl(account.BaseUrl, calendarEvent.EventId.ToString());

        await _client.UpdateEventAsync(
                eventUrl,
                ics,
                account.AccessToken,
                cancellationToken);
    }
}
