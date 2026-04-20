using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.GenerateTelegramLinkToken;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class GenerateTelegramTokenTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private async Task<User> CreateUserAsync()
    {
        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = $"user_{Guid.NewGuid()}",
            Email = $"{Guid.NewGuid()}@mail.com"
        };

        await userManager.CreateAsync(user, "Password123!");
        return user;
    }

    [Fact]
    public async Task Should_Generate_Token()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        var token = await mediator.Send(new GenerateTelegramLinkTokenCommand(user.Id));

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(32, token.Length);
    }

    [Fact]
    public async Task Should_Save_Token_And_Expiration()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        var token = await mediator.Send(new GenerateTelegramLinkTokenCommand(user.Id));

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var updated = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.Equal(token, updated!.TelegramToken);
        Assert.True(updated.TelegramTokenExpiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new GenerateTelegramLinkTokenCommand(Guid.NewGuid())));
    }
}
