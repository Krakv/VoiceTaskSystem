using MediatR;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.ApiGateway.DTOs.CalendarEvent;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.Calendar.Application.Features.CalendarEvent.DeleteCalendarEvent;
using TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvent;
using TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;
using TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Interfaces;

namespace TaskManager.ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/calendar-events")]
public class CalendarEventsController(IMediator mediator, ICurrentUser user) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _user = user;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCalendarEventDto dto)
    {
        var command = new CreateCalendarEventCommand(
            OwnerId: _user.UserId,
            Title: dto.Title,
            StartTime: DateTimeOffset.Parse(dto.StartTime, CultureInfo.InvariantCulture),
            EndTime: DateTimeOffset.Parse(dto.EndTime, CultureInfo.InvariantCulture),
            Location: dto.Location,
            TaskId: string.IsNullOrWhiteSpace(dto.TaskId) ? null : Guid.Parse(dto.TaskId),
            ExternalAccountId: string.IsNullOrWhiteSpace(dto.ExternalAccountId) ? null : Guid.Parse(dto.ExternalAccountId)
        );

        var result = await _mediator.Send(command);
        return CreatedResponse($"api/v1/calendar-events/{result}", result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetCalendarEventsQuery(_user.UserId));
        return Success(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCalendarEventQuery(_user.UserId, id));
        return Success(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCalendarEventDto dto)
    {
        var command = new UpdateCalendarEventCommand(
            OwnerId: _user.UserId,
            CalendarEventId: id,
            Title: dto.Title,
            StartTime: DateTimeOffset.Parse(dto.StartTime, CultureInfo.InvariantCulture),
            EndTime: DateTimeOffset.Parse(dto.EndTime, CultureInfo.InvariantCulture),
            Location: dto.Location,
            TaskId: string.IsNullOrWhiteSpace(dto.TaskId) ? null : Guid.Parse(dto.TaskId),
            ExternalAccountId: string.IsNullOrWhiteSpace(dto.ExternalAccountId) ? null : Guid.Parse(dto.ExternalAccountId)
        );

        await _mediator.Send(command);
        return Success(new { });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCalendarEventCommand(_user.UserId, id));
        return Success(new { });
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}