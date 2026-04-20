using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Interfaces;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.DeleteExternalCalendar;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.ExchangeOAuthCode;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetAuthorizeUrl;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetExternalCalendars;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Calendar.ExternalCalendarFeature;

public class ExternalCalendarTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private async Task<Guid> CreateAccountAsync(Guid userId)
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var account = new ExternalCalendarAccount
        {
            OwnerId = userId,
            BaseUrl = $"https://caldav.yandex.ru/calendars/{Guid.NewGuid()}/events-default/",
            AccessToken = "access",
            RefreshToken = "refresh",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        await context.ExternalCalendarAccount.AddAsync(account);
        await context.SaveChangesAsync();

        return account.ExternalCalendarAccountId;
    }

    [Fact]
    public async Task GetExternalCalendars_Should_Return_Only_Owner_Accounts()
    {
        var userId1 = await fixture.CreateUserAsync();
        var userId2 = await fixture.CreateUserAsync();

        await CreateAccountAsync(userId1);
        await CreateAccountAsync(userId1);
        await CreateAccountAsync(userId2);

        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetExternalCalendarsQuery(userId1));

        Assert.All(result, x => Assert.NotEqual(Guid.Empty, x.ExternalCalendarAccountId));
        Assert.True(result.Count >= 2);
        Assert.DoesNotContain(result, x => x.BaseUrl.Contains(userId2.ToString()));
    }

    [Fact]
    public async Task GetExternalCalendars_Should_Return_Empty_When_No_Accounts()
    {
        var userId = await fixture.CreateUserAsync();

        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetExternalCalendarsQuery(userId));

        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteExternalCalendar_Should_Remove_Account()
    {
        var userId = await fixture.CreateUserAsync();
        var accountId = await CreateAccountAsync(userId);

        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new DeleteExternalCalendarCommand(userId, accountId));

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var account = await context.ExternalCalendarAccount
            .FirstOrDefaultAsync(x => x.ExternalCalendarAccountId == accountId);

        Assert.Null(account);
    }

    [Fact]
    public async Task DeleteExternalCalendar_Should_Throw_When_Not_Found()
    {
        var userId = await fixture.CreateUserAsync();

        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new DeleteExternalCalendarCommand(userId, Guid.NewGuid())));
    }

    [Fact]
    public async Task DeleteExternalCalendar_Should_Throw_When_Wrong_Owner()
    {
        var userId1 = await fixture.CreateUserAsync();
        var userId2 = await fixture.CreateUserAsync();
        var accountId = await CreateAccountAsync(userId1);

        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new DeleteExternalCalendarCommand(userId2, accountId)));
    }

    [Fact]
    public async Task GetAuthorizeUrl_Should_Return_Url_With_State()
    {
        var userId = await fixture.CreateUserAsync();

        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var url = await mediator.Send(new GetAuthorizeUrlQuery(userId));

        Assert.Contains("response_type=code", url);
        Assert.Contains("client_id=", url);
        Assert.Contains("state=", url);
    }

    [Fact]
    public async Task ExchangeOAuthCode_Should_Create_Account()
    {
        var userId = await fixture.CreateUserAsync();

        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var stateService = scope.ServiceProvider.GetRequiredService<IStateService>();

        var state = stateService.Generate(userId);
        await mediator.Send(new ExchangeOAuthCodeCommand("any_code", state));

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var account = await context.ExternalCalendarAccount
            .FirstOrDefaultAsync(x => x.OwnerId == userId);

        Assert.NotNull(account);
        Assert.Equal("fake_access_token", account.AccessToken);
        Assert.Equal("fake_refresh_token", account.RefreshToken);
        Assert.Contains("testuser@yandex.ru", account.BaseUrl);
    }

    [Fact]
    public async Task ExchangeOAuthCode_Should_Update_Existing_Account()
    {
        var userId = await fixture.CreateUserAsync();

        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var stateService = scope.ServiceProvider.GetRequiredService<IStateService>();

        var state = stateService.Generate(userId);

        await mediator.Send(new ExchangeOAuthCodeCommand("code1", state));

        await mediator.Send(new ExchangeOAuthCodeCommand("code2", state));

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var accounts = await context.ExternalCalendarAccount
            .Where(x => x.OwnerId == userId)
            .ToListAsync();

        Assert.Single(accounts);
    }
}
