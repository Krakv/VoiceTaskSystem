using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.DeleteUser;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class DeleteUserTests(TestFixture fixture) : IClassFixture<TestFixture>
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
            Name = "Test User"
        };

        await userManager.CreateAsync(user, "Password123!");
        return user;
    }

    [Fact]
    public async Task Should_Delete_User_Softly()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        var result = await mediator.Send(new DeleteUserCommand(user.Id));

        Assert.True(result);

        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var deletedUser = await context.Users.FindAsync(user.Id);

        Assert.NotNull(deletedUser);
        Assert.True(deletedUser!.IsDeleted);
        Assert.NotNull(deletedUser.DeletedAt);
    }

    [Fact]
    public async Task Should_Anonymize_User_Data()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        await mediator.Send(new DeleteUserCommand(user.Id));

        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var deletedUser = await context.Users.FindAsync(user.Id);

        Assert.StartsWith("deleted_", deletedUser!.Email);
        Assert.StartsWith("deleted_", deletedUser.UserName);
        Assert.Equal("Deleted user", deletedUser.Name);
        Assert.Null(deletedUser.PhoneNumber);
        Assert.StartsWith("DELETED_", deletedUser.NormalizedEmail);
        Assert.StartsWith("DELETED_", deletedUser.NormalizedUserName);
    }

    [Fact]
    public async Task Should_Remove_Related_Data()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        User user;
        using (var scope = _provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            user = new User
            {
                Id = Guid.NewGuid(),
                UserName = $"user_{Guid.NewGuid()}",
                Email = $"{Guid.NewGuid()}@mail.com"
            };

            await userManager.CreateAsync(user, "Password123!");

            context.TaskItems.Add(new TaskItem
            {
                TaskId = Guid.NewGuid(),
                OwnerId = user.Id,
                Title = "Test task"
            });

            context.ExternalCalendarAccount.Add(new ExternalCalendarAccount
            {
                ExternalCalendarAccountId = Guid.NewGuid(),
                OwnerId = user.Id,
                BaseUrl = "https://calendar.example.com",
            });

            await context.SaveChangesAsync();
        }

        await mediator.Send(new DeleteUserCommand(user.Id));

        using (var scope = _provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var tasks = context.TaskItems.Where(x => x.OwnerId == user.Id);
            var accounts = context.ExternalCalendarAccount.Where(x => x.OwnerId == user.Id);

            Assert.Empty(tasks);
            Assert.Empty(accounts);
        }
    }

    [Fact]
    public async Task Should_Return_False_When_User_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new DeleteUserCommand(Guid.NewGuid()));

        Assert.False(result);
    }
}
