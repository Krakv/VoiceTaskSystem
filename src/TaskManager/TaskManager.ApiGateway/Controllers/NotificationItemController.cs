using MediatR;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotifications;
using TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Notifications.Application.Features.NotificationFeature.DeleteNotification;
using TaskManager.ApiGateway.DTOs.Notification;
using TaskManager.Shared.Interfaces;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;

namespace TaskManager.ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/notifications")]
public class NotificationItemController(IMediator mediator, ICurrentUser user) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _user = user;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
    {
        var command = new CreateNotificationCommand(
            OwnerId: _user.UserId,
            ServiceId: dto.ServiceId,
            Description: dto.Description,
            ScheduledAt: DateTimeOffset.Parse(dto.ScheduledAt, CultureInfo.InvariantCulture),
            TaskId: string.IsNullOrWhiteSpace(dto.TaskId)
                ? null
                : Guid.Parse(dto.TaskId)
        );

        var result = await _mediator.Send(command);

        return CreatedResponse($"api/v1/notifications/{result}", result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetNotificationsQuery(_user.UserId));
        return Success(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetNotificationQuery(_user.UserId, id));
        return Success(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateNotificationDto dto)
    {
        var updateNotifCommand = new UpdateNotificationCommand
        (
            _user.UserId,
            Guid.Parse(id),
            dto.Description,
            DateTimeOffset.Parse(dto.ScheduledAt, CultureInfo.InvariantCulture)
        );

        await _mediator.Send(updateNotifCommand);
        return Success(new {});
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteNotificationCommand(_user.UserId, id));
        return Success(new { });
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}
