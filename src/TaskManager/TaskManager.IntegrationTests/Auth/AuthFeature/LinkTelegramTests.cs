using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.GenerateTelegramLinkToken;
using TaskManager.Auth.Application.Features.Auth.LinkTelegram;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class LinkTelegramTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private async Task<(User user, string token)> CreateUserWithToken()
    {
        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = $"user_{Guid.NewGuid()}",
            Email = $"{Guid.NewGuid()}@mail.com"
        };

        await userManager.CreateAsync(user, "Password123!");

        var token = await mediator.Send(new GenerateTelegramLinkTokenCommand(user.Id));

        return (user, token);
    }

    [Fact]
    public async Task Should_Link_Telegram()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (user, token) = await CreateUserWithToken();

        await mediator.Send(new LinkTelegramCommand(token, 123456789));

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var updated = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.Equal(123456789, updated!.TelegramChatId);
        Assert.Null(updated.TelegramToken);
        Assert.Null(updated.TelegramTokenExpiresAt);
    }

    [Fact]
    public async Task Should_Throw_When_Token_Invalid()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new LinkTelegramCommand("invalid", 123)));
    }

    [Fact]
    public async Task Should_Throw_When_Token_Expired()
    {
        var userId = await fixture.CreateUserAsync();

        using (var scope1 = _provider.CreateScope())
        {
            var mediator = scope1.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new GenerateTelegramLinkTokenCommand(userId));
        }

        using (var scope2 = _provider.CreateScope())
        {
            var context = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbUser = await context.Users.FirstAsync(x => x.Id == userId);
            dbUser.TelegramTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-240);
            await context.SaveChangesAsync();
        }

        using (var scope3 = _provider.CreateScope())
        {
            var mediator = scope3.ServiceProvider.GetRequiredService<IMediator>();
            var token = await scope3.ServiceProvider
                .GetRequiredService<AppDbContext>()
                .Users
                .Where(x => x.Id == userId)
                .Select(x => x.TelegramToken)
                .FirstAsync();

            await Assert.ThrowsAsync<ValidationAppException>(() =>
                mediator.Send(new LinkTelegramCommand(token!, 123)));
        }
    }
}
