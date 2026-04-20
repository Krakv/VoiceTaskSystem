using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.UnlinkTelegramChat;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class UnlinkTelegramTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private async Task<User> CreateUserWithTelegram()
    {
        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = $"user_{Guid.NewGuid()}",
            Email = $"{Guid.NewGuid()}@mail.com",
            TelegramChatId = 123456
        };

        await userManager.CreateAsync(user, "Password123!");
        return user;
    }

    [Fact]
    public async Task Should_Unlink_Telegram()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserWithTelegram();

        await mediator.Send(new UnlinkTelegramCommand(user.Id));

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var updated = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.Null(updated!.TelegramChatId);
        Assert.Null(updated.TelegramToken);
        Assert.Null(updated.TelegramTokenExpiresAt);
    }

    [Fact]
    public async Task Should_Throw_When_Not_Linked()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = $"user_{Guid.NewGuid()}",
            Email = $"{Guid.NewGuid()}@mail.com"
        };

        await userManager.CreateAsync(user, "Password123!");

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new UnlinkTelegramCommand(user.Id)));
    }
}
