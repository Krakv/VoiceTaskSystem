using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManager.Auth.Config;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.DeleteExternalCalendar;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.ExchangeOAuthCode;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetAuthorizeUrl;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetExternalCalendars;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.ApiGateway.Controllers;

[ApiController]
[Route("api/v1/oauth")]
public class OAuthController(IMediator mediator, IOptions<FrontendOptions> options) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly FrontendOptions _options = options.Value;

    [HttpGet("yandex/callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, string state)
    {
        await _mediator.Send(new ExchangeOAuthCodeCommand(code, state));

        return Redirect(_options.Url);
    }

    [Authorize]
    [HttpGet("yandex/connect")]
    public async Task<IActionResult> Connect()
    {
        var url = await _mediator.Send(new GetAuthorizeUrlQuery());

        return Success(url);
    }

    [Authorize]
    [HttpDelete("accounts/{id:guid}")]
    public async Task<IActionResult> Disconnect(string id)
    {
        await _mediator.Send(new DeleteExternalCalendarCommand(id));

        return Success(new { });
    }

    [Authorize]
    [HttpGet("accounts")]
    public async Task<IActionResult> GetCalendars()
    {
        var cals = await _mediator.Send(new GetExternalCalendarsQuery());

        return Success(cals);
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}

