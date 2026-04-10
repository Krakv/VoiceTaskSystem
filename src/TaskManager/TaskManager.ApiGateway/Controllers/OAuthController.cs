using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Auth.Application.Features.OAuth.ExchangeOAuthCode;
using TaskManager.Auth.Application.Features.OAuth.GetAuthorizeUrl;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.ApiGateway.Controllers;

[ApiController]
[Route("api/v1/oauth")]
public class OAuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("yandex/callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, string state)
    {
        await _mediator.Send(new ExchangeOAuthCodeCommand(code, state));

        return Success(new { });
    }

    [Authorize]
    [HttpGet("yandex/connect")]
    public async Task<IActionResult> Connect()
    {
        var url = await _mediator.Send(new GetAuthorizeUrlQuery());

        return Redirect(url);
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}

