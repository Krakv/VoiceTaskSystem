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
[Produces("application/json")]
public class NotificationItemController(IMediator mediator, ICurrentUser user) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _user = user;

    /// <summary>
    /// Создать уведомление
    /// </summary>
    /// <remarks>
    /// Планирует новое уведомление для пользователя. Поддерживает привязку к задаче.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Получить все уведомления пользователя
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SuccessResponse<List<GetNotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetNotificationsQuery(_user.UserId));
        return Success(result);
    }

    /// <summary>
    /// Получить уведомление по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<GetNotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetNotificationQuery(_user.UserId, id));
        return Success(result);
    }

    /// <summary>
    /// Обновить уведомление
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNotificationDto dto)
    {
        var updateNotifCommand = new UpdateNotificationCommand(
            _user.UserId,
            id,
            dto.Description,
            DateTimeOffset.Parse(dto.ScheduledAt, CultureInfo.InvariantCulture)
        );

        await _mediator.Send(updateNotifCommand);

        return Success(new { isUpdated = true });
    }

    /// <summary>
    /// Удалить уведомление
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteNotificationCommand(_user.UserId, id));

        return Success(new { isDeleted = true });
    }

    /// <summary>
    /// Унифицированный успешный ответ API
    /// </summary>
    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));

    /// <summary>
    /// Ответ при создании ресурса (201 Created)
    /// </summary>
    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));
}