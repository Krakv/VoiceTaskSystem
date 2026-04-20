using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Auth.Application.Features.Auth.Login;
using TaskManager.Auth.Application.Features.Auth.RegisterUser;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.ApiGateway.Controllers.Public;

[ApiController]
[Route("api/v1/auth")]
public class AuthPublicController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedResponse(result.ToString(), $"api/v1/auth/login/{result}");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var token = await _mediator.Send(command);
        return Success(token);
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(T data, string route) where T : class =>
        Created(route, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}

