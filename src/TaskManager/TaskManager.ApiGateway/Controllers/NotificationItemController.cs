using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.ApiGateway.DTOs;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotifications;
using TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Notifications.Application.Features.NotificationFeature.DeleteNotification;

namespace TaskManager.ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/notifications")]
public class NotificationItemController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedResponse($"api/v1/notifications/{result}", result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetNotificationsQuery());
        return Success(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _mediator.Send(new GetNotificationQuery(id));
        return Success(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateNotificationDto dto)
    {
        var updateNotifCommand = new UpdateNotificationCommand
        (
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
        await _mediator.Send(new DeleteNotificationCommand(id));
        return Success(new { });
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}
