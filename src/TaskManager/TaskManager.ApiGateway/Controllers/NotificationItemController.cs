using MediatR;
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
        var result = await _mediator.Send(new CreateNotificationCommand(_user.UserId.ToString(), dto.ServiceId, dto.Description, dto.ScheduledAt, dto.TaskId));
        return CreatedResponse($"api/v1/notifications/{result}", result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetNotificationsQuery(_user.UserId.ToString()));
        return Success(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _mediator.Send(new GetNotificationQuery(_user.UserId.ToString(), id));
        return Success(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateNotificationDto dto)
    {
        var updateNotifCommand = new UpdateNotificationCommand
        (
            _user.UserId.ToString(),
            id,
            dto.Description,
            dto.ScheduledAt
        );

        await _mediator.Send(updateNotifCommand);
        return Success(new {});
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new DeleteNotificationCommand(_user.UserId.ToString(), id));
        return Success(new { });
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}
