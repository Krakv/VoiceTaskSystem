using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.RegisterUser;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class RegisterUserTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    [Fact]
    public async Task Should_Register_User()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var command = new RegisterUserCommand(
            UserName: $"user_{Guid.NewGuid()}",
            Email: $"test_{Guid.NewGuid()}@mail.com",
            Password: "Password123!",
            Name: "John Doe"
        );

        var userId = await mediator.Send(command);

        var user = await context.Users.FindAsync(userId);

        Assert.NotNull(user);
        Assert.Equal(userId, user!.Id);
        Assert.Equal(command.UserName, user.UserName);
        Assert.Equal(command.Email, user.Email);
        Assert.Equal("John Doe", user.Name);
    }

    [Fact]
    public async Task Should_Return_New_Guid_On_Register()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var command = new RegisterUserCommand(
            UserName: $"user_{Guid.NewGuid()}",
            Email: $"test_{Guid.NewGuid()}@mail.com",
            Password: "Password123!",
            Name: "Test"
        );

        var userId = await mediator.Send(command);

        Assert.NotEqual(Guid.Empty, userId);
    }

    [Fact]
    public async Task Should_Throw_When_Email_Already_Exists()
    {
        var email = $"duplicate_{Guid.NewGuid()}@mail.com";

        using (var scope1 = _provider.CreateScope())
        {
            var mediator = scope1.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new RegisterUserCommand(
                UserName: $"user1_{Guid.NewGuid()}",
                Email: email,
                Password: "Password123!",
                Name: "User1"
            ));
        }

        using (var scope2 = _provider.CreateScope())
        {
            var mediator = scope2.ServiceProvider.GetRequiredService<IMediator>();
            await Assert.ThrowsAsync<ValidationAppException>(() =>
                mediator.Send(new RegisterUserCommand(
                    UserName: $"user2_{Guid.NewGuid()}",
                    Email: email,
                    Password: "Password123!",
                    Name: "User2"
                )));
        }
    }

    [Fact]
    public async Task Should_Throw_When_Password_Invalid()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var command = new RegisterUserCommand(
            UserName: $"user_{Guid.NewGuid()}",
            Email: $"test_{Guid.NewGuid()}@mail.com",
            Password: "123",
            Name: "Test"
        );

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(command));
    }

    [Fact]
    public async Task Should_Normalize_UserName_And_Email()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var command = new RegisterUserCommand(
            UserName: "TestUser",
            Email: "Test@Mail.com",
            Password: "Password123!",
            Name: "Test"
        );

        var userId = await mediator.Send(command);

        var user = await context.Users.FindAsync(userId);

        Assert.Equal("TESTUSER", user!.NormalizedUserName);
        Assert.Equal("TEST@MAIL.COM", user.NormalizedEmail);
    }
}
