using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.UpdateUserProfile;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class UpdateUserProfileTests(TestFixture fixture) : IClassFixture<TestFixture>
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
            Email = $"{Guid.NewGuid()}@mail.com",
            Name = "Old Name"
        };

        await userManager.CreateAsync(user, "Password123!");
        return user;
    }

    [Fact]
    public async Task Should_Update_Name()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        await mediator.Send(new UpdateUserProfileCommand(
            user.Id,
            Name: "New Name",
            Email: null
        ));

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var updated = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.Equal("New Name", updated!.Name);
    }

    [Fact]
    public async Task Should_Update_Email()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        var newEmail = $"new_{Guid.NewGuid()}@mail.com";

        await mediator.Send(new UpdateUserProfileCommand(
            user.Id,
            Name: null,
            Email: newEmail
        ));

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var updated = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.Equal(newEmail, updated!.Email);
    }

    [Fact]
    public async Task Should_Update_Both_Name_And_Email()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        var newEmail = $"new_{Guid.NewGuid()}@mail.com";

        await mediator.Send(new UpdateUserProfileCommand(
            user.Id,
            Name: "Updated Name",
            Email: newEmail
        ));

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var updated = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.Equal("Updated Name", updated!.Name);
        Assert.Equal(newEmail, updated.Email);
    }

    [Fact]
    public async Task Should_Not_Change_Email_When_Same()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        await mediator.Send(new UpdateUserProfileCommand(
            user.Id,
            Name: null,
            Email: user.Email
        ));

        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var updated = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.Equal(user.Email, updated!.Email);
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new UpdateUserProfileCommand(
                Guid.NewGuid(),
                "Name",
                "test@mail.com"
            )));
    }

    [Fact]
    public async Task Should_Throw_When_User_Deleted()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        using (var scope = _provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<
                TaskManager.Repository.Context.AppDbContext>();

            user.IsDeleted = true;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new UpdateUserProfileCommand(
                user.Id,
                "New Name",
                null
            )));
    }

    [Fact]
    public async Task Should_Throw_When_Email_Invalid()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new UpdateUserProfileCommand(
                user.Id,
                null,
                "invalid-email"
            )));

        Assert.Equal("INVALID_PARAMS", ex.ErrorCode);
        Assert.True(ex.Errors?.ContainsKey("Email"));
    }
}
