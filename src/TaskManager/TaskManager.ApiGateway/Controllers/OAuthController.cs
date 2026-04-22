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
using TaskManager.Shared.Interfaces;

namespace TaskManager.ApiGateway.Controllers;

[ApiController]
[Route("api/v1/oauth")]
[Produces("application/json")]
public class OAuthController(IMediator mediator, IOptions<FrontendOptions> options, ICurrentUser user) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _user = user;
    private readonly FrontendOptions _options = options.Value;

    /// <summary>
    /// OAuth callback от Yandex. Обменивает authorization code на токены и привязывает аккаунт.
    /// После успешной авторизации выполняется редирект на фронтенд.
    /// </summary>
    [HttpGet("yandex/callback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {
        await _mediator.Send(new ExchangeOAuthCodeCommand(code, state));

        return Redirect(_options.Url);
    }

    /// <summary>
    /// Получить ссылку для подключения Yandex Calendar (OAuth authorize URL)
    /// </summary>
    [Authorize]
    [HttpGet("yandex/connect")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Connect()
    {
        var url = await _mediator.Send(new GetAuthorizeUrlQuery(_user.UserId));

        return Success(url);
    }

    /// <summary>
    /// Отключить внешний календарь (удалить OAuth привязку)
    /// </summary>
    [Authorize]
    [HttpDelete("accounts/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Disconnect(Guid id)
    {
        await _mediator.Send(new DeleteExternalCalendarCommand(_user.UserId, id));

        return Success(new { isDeleted = true });
    }

    /// <summary>
    /// Получить список подключенных внешних календарей пользователя
    /// </summary>
    [Authorize]
    [HttpGet("accounts")]
    [ProducesResponseType(typeof(SuccessResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCalendars()
    {
        var cals = await _mediator.Send(new GetExternalCalendarsQuery(_user.UserId));

        return Success(cals);
    }

    /// <summary>
    /// Унифицированный успешный ответ API
    /// </summary>
    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));
}