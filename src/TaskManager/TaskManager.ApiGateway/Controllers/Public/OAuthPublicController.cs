using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManager.Auth.Config;
using TaskManager.Calendar.Application.Features.ExternalCalendarAccount.ExchangeOAuthCode;
using TaskManager.Shared.Interfaces;

namespace TaskManager.ApiGateway.Controllers.Public;

[ApiController]
[Route("api/v1/oauth")]
[Produces("application/json")]
public class OAuthPublicController(IMediator mediator, IOptions<FrontendOptions> options) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
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
}
