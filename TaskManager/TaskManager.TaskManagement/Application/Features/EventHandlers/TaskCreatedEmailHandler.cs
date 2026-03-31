using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using TaskManager.Application.Domain.Entities;
using TaskManager.Application.Services.Interfaces;
using TaskManager.TaskManagement.Application.Features.Events;

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
