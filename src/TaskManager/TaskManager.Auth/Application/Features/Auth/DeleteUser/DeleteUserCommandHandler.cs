using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Auth.Application.Features.Auth.DeleteUser;

public sealed class DeleteUserCommandHandler(UserManager<User> userManager, AppDbContext dbContext, ICurrentUser currentUser, ILogger<DeleteUserCommandHandler> logger) : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<DeleteUserCommandHandler> _logger = logger;

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var user = await _userManager.Users
            .Include(u => u.Tasks)
            .Include(u => u.CommandRequests)
            .Include(u => u.ExternalCalendarAccounts)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Failed to delete user {UserId}, user not found", userId);
            return false;
        }

        user.IsDeleted = true;
        user.DeletedAt = DateTimeOffset.UtcNow;

        user.Email = $"deleted_{user.Id}@local.deleted";
        user.UserName = $"deleted_{user.Id}";
        user.Name = "Deleted user";
        user.PhoneNumber = null;
        user.NormalizedEmail = null;
        user.NormalizedUserName = null;

        _dbContext.ExternalCalendarAccount.RemoveRange(user.ExternalCalendarAccounts);
        _dbContext.TaskItems.RemoveRange(user.Tasks);

        await _userManager.UpdateAsync(user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
