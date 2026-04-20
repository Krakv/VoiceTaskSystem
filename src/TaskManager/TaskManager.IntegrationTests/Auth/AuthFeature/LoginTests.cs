using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManager.Auth.Application.Features.Auth.Login;
using TaskManager.Auth.Config;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class LoginTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private async Task<(string username, string password)> CreateUser()
    {
        var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<
            Microsoft.AspNetCore.Identity.UserManager<TaskManager.Shared.Domain.Entities.User>>();

        var username = $"user_{Guid.NewGuid()}";
        var email = $"{username}@test.com";
        var password = "Password123!";

        var user = new TaskManager.Shared.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            UserName = username,
            Email = email
        };

        await userManager.CreateAsync(user, password);

        return (username, password);
    }

    [Fact]
    public async Task Should_Login_And_Return_Jwt()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (username, password) = await CreateUser();

        var command = new LoginCommand(username, password);

        var token = await mediator.Send(command);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Should_Contain_Valid_Claims()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (username, password) = await CreateUser();

        var command = new LoginCommand(username, password);

        var token = await mediator.Send(command);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Contains(jwt.Claims, c => c.Type == "unique_name" && c.Value == username);
        Assert.Contains(jwt.Claims, c => c.Type == "role" && c.Value == "User");
        Assert.Contains(jwt.Claims, c => c.Type == "nameid");
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var command = new LoginCommand("unknown_user", "Password123!");

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(command));
    }

    [Fact]
    public async Task Should_Throw_When_Password_Invalid()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (username, _) = await CreateUser();

        var command = new LoginCommand(username, "WrongPassword");

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(command));
    }

    [Fact]
    public async Task Should_Set_Expiration_Correctly()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var settings = _provider.GetRequiredService<IOptions<JwtSettings>>().Value;

        var (username, password) = await CreateUser();

        var token = await mediator.Send(new LoginCommand(username, password));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var expected = DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes);

        Assert.True(jwt.ValidTo <= expected.AddSeconds(5));
    }
}
