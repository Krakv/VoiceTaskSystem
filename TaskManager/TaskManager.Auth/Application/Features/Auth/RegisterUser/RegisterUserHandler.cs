using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManager.Application.Domain.Entities;
using TaskManager.Exceptions;

namespace TaskManager.Application.Features.Auth.RegisterUser;

public class RegisterUserHandler(UserManager<User> userManager, ILogger<RegisterUserHandler> logger) : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly ILogger<RegisterUserHandler> _logger = logger;

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            NormalizedUserName = request.UserName.ToUpper(),
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            Name = request.Name,
            CreatedAt = DateTimeOffset.UtcNow,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ValidationAppException("REGISTRATION_FAILED", errors);
        }

        _logger.LogInformation("User registered with id {UserId}", user.Id);
        return user.Id;
    }
}

