using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.ApiGateway.DTOs.CalendarEvent;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.Calendar.Application.Features.CalendarEvent.DeleteCalendarEvent;
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
    public async Task<IActionResult> Create(CreateCalendarEventDto dto)
    {
        var command = new CreateCalendarEventCommand(
            _user.UserId.ToString(),
            dto.Title,
            dto.StartTime,
            dto.EndTime,
            dto.Location,
            dto.TaskId,
            dto.ExternalAccountId
            );

        var result = await _mediator.Send(command);
        return CreatedResponse("api/v1/calendar-events", result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetCalendarEventsQuery(_user.UserId.ToString()));
        return Success(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCalendarEventDto dto)
    {
        var command = new UpdateCalendarEventCommand(
            _user.UserId.ToString(), 
            dto.CalendarEventId,
            dto.Title,
            dto.StartTime, 
            dto.EndTime, 
            dto.Location, 
            dto.TaskId, 
            dto.ExternalAccountId
            );

        await _mediator.Send(command);
        return Success(new { });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new DeleteCalendarEventCommand(_user.UserId.ToString(), id));
        return Success(new { });
    }


    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}
