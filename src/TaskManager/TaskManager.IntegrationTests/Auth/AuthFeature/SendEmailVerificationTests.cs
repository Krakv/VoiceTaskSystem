using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.SendEmailVerification;
using TaskManager.IntegrationTests.Factories;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.IntegrationTests.Auth.AuthFeature;

public class SendEmailVerificationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private async Task<User> CreateUserAsync(bool confirmed = false)
    {
        using var scope = _provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = $"user_{Guid.NewGuid()}",
            Email = $"{Guid.NewGuid()}@mail.com",
            EmailConfirmed = confirmed
        };

        await userManager.CreateAsync(user, "Password123!");
        return user;
    }

    [Fact]
    public async Task Should_Send_Email_With_Confirmation_Link()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var emailService = (FakeEmailService)_provider.GetRequiredService<IEmailService>();

        var user = await CreateUserAsync();

        await mediator.Send(new SendEmailVerificationCommand(user.Id));

        Assert.Single(emailService.SentEmails);

        var (_, Subject, Body) = emailService.SentEmails[0];

        Assert.Contains("Подтверждение email", Subject);
        Assert.Contains("email-confirm", Body);
        Assert.Contains("token=", Body);
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Found()
    {
        var mediator = _provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new SendEmailVerificationCommand(Guid.NewGuid())));
    }

    [Fact]
    public async Task Should_Throw_When_Email_Already_Confirmed()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var user = await CreateUserAsync(confirmed: true);

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new SendEmailVerificationCommand(user.Id)));
    }
}
