using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.Calendar.Application.Features.CalendarEvent.DeleteCalendarEvent;
using TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;
using TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/calendar-events")]
public class CalendarEventsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator; 

    [HttpPost]
    public async Task<IActionResult> Create(CreateCalendarEventCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedResponse("api/v1/calendar-events", result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetCalendarEventsQuery());
        return Success(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCalendarEventCommand command)
    {
        await _mediator.Send(command);
        return Success(new { });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCalendarEventCommand(id));
        return Success(new { });
    }


    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}
