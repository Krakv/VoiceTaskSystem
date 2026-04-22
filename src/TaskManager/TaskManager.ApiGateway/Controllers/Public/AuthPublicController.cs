using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Auth.Application.Features.Auth.Login;
using TaskManager.Auth.Application.Features.Auth.RegisterUser;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.ApiGateway.Controllers.Public;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthPublicController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <remarks>
    /// Создаёт пользователя и возвращает его идентификатор.
    /// После регистрации можно использовать этот id для авторизации.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);

        return CreatedResponse(result.ToString(), $"api/v1/auth/login/{result}");
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <remarks>
    /// Возвращает JWT токен для доступа к защищённым endpoint-ам.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SuccessResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var token = await _mediator.Send(command);
        return Success(token);
    }

    /// <summary>
    /// Унифицированный успешный ответ API (200 OK)
    /// </summary>
    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));

    /// <summary>
    /// Ответ при создании ресурса (201 Created)
    /// </summary>
    private CreatedResult CreatedResponse<T>(T data, string route) where T : class =>
        Created(route, new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));
}