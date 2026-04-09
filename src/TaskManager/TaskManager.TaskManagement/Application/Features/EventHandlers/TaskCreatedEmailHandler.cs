using Microsoft.AspNetCore.Identity;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Shared.EventHandlers;
using Microsoft.Extensions.Logging;
using TaskManager.Shared.Events;

namespace TaskManager.TaskManagement.Application.Features.EventHandlers;

public class TaskCreatedEmailHandler(ILogger<BaseEventHandler<TaskCreatedEvent>> logger, IEmailService emailService, UserManager<User> userManager) : BaseEventHandler<TaskCreatedEvent>(logger)
{
    private readonly IEmailService _emailService = emailService;
    private readonly UserManager<User> _userManager = userManager;

    protected override async Task Process(TaskCreatedEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(notification.UserId.ToString());
        if (user == null) return;
        if (user.Email == null) return;
        if (!user.EmailConfirmed) return;

        await _emailService.Send(
            user.Email,
            "Создана новая задача",
            $"Задача '{notification.Title}' была создана с ID {notification.TaskId}");
    }
}
