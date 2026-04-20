using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.ConfirmEmail;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class ConfirmEmailTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private async Task<(User user, string token)> CreateUserWithToken()
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

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        return (user, Uri.EscapeDataString(token));
    }

    [Fact]
    public async Task Should_Confirm_Email()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var (user, token) = await CreateUserWithToken();

        await mediator.Send(new ConfirmEmailCommand(user.Id, token));

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var updated = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.True(updated!.EmailConfirmed);
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new ConfirmEmailCommand(Guid.NewGuid(), "token")));
    }

    [Fact]
    public async Task Should_Throw_When_Already_Confirmed()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (user, token) = await CreateUserWithToken();

        await mediator.Send(new ConfirmEmailCommand(user.Id, token));

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new ConfirmEmailCommand(user.Id, token)));
    }

    [Fact]
    public async Task Should_Throw_When_Token_Invalid()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (user, _) = await CreateUserWithToken();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new ConfirmEmailCommand(user.Id, "invalid_token")));
    }
}
