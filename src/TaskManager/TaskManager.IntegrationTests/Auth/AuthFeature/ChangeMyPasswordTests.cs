using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.ChangeMyPassword;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class ChangeMyPasswordTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private async Task<(User user, string password)> CreateUserAsync()
    {
        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var password = "Password123!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = $"user_{Guid.NewGuid()}",
            Email = $"{Guid.NewGuid()}@mail.com"
        };

        await userManager.CreateAsync(user, password);

        return (user, password);
    }

    [Fact]
    public async Task Should_Change_Password()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (user, currentPassword) = await CreateUserAsync();

        var newPassword = "NewPassword123!";

        var result = await mediator.Send(new ChangeMyPasswordCommand(
            user.Id,
            currentPassword,
            newPassword
        ));

        Assert.True(result);

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var refreshedUser = await userManager.FindByIdAsync(user.Id.ToString());

        var validOld = await userManager.CheckPasswordAsync(refreshedUser!, currentPassword);
        var validNew = await userManager.CheckPasswordAsync(refreshedUser!, newPassword);

        Assert.False(validOld);
        Assert.True(validNew);
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new ChangeMyPasswordCommand(
                Guid.NewGuid(),
                "Password123!",
                "NewPassword123!"
            )));
    }

    [Fact]
    public async Task Should_Throw_When_User_Deleted()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (user, password) = await CreateUserAsync();

        using (var scope = _provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<
                TaskManager.Repository.Context.AppDbContext>();

            user.IsDeleted = true;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new ChangeMyPasswordCommand(
                user.Id,
                password,
                "NewPassword123!"
            )));
    }

    [Fact]
    public async Task Should_Throw_When_CurrentPassword_Wrong()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (user, _) = await CreateUserAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new ChangeMyPasswordCommand(
                user.Id,
                "WrongPassword",
                "NewPassword123!"
            )));

        Assert.Equal("INVALID_PARAMS", ex.ErrorCode);
    }

    [Fact]
    public async Task Should_Throw_When_NewPassword_Invalid()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var (user, password) = await CreateUserAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new ChangeMyPasswordCommand(
                user.Id,
                password,
                "123"
            )));

        Assert.Equal("INVALID_PARAMS", ex.ErrorCode);
    }
}
