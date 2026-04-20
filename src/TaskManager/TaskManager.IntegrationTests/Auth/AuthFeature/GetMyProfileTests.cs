using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.GetMyProfile;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class GetMyProfileTests(TestFixture fixture) : IClassFixture<TestFixture>
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
            Name = "Test User",
            EmailConfirmed = true,
            TelegramChatId = 123456
        };

        await userManager.CreateAsync(user, "Password123!");
        return user;
    }

    [Fact]
    public async Task Should_Return_Profile()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync();

        var query = new GetMyProfileQuery(user.Id);

        var result = await mediator.Send(query);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.True(result.EmailVerified);
        Assert.Equal(123456, result.TelegramChatId);
    }

    [Fact]
    public async Task Should_Include_ExternalAccounts()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var user = await CreateUserAsync();

        var accountId = Guid.NewGuid();

        context.ExternalCalendarAccount.Add(new ExternalCalendarAccount
        {
            ExternalCalendarAccountId = accountId,
            OwnerId = user.Id,
            BaseUrl = "https://example.com/calendar",
        });

        await context.SaveChangesAsync();

        var result = await mediator.Send(new GetMyProfileQuery(user.Id));

        Assert.Contains(accountId, result.ExternalCalendarAccountIds);
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        var query = new GetMyProfileQuery(Guid.NewGuid());

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(query));
    }

    [Fact]
    public async Task Should_Throw_When_User_Deleted()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var context = _provider.GetRequiredService<AppDbContext>();

        var user = await CreateUserAsync();

        user.IsDeleted = true;
        context.Users.Update(user);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new GetMyProfileQuery(user.Id)));
    }
}
