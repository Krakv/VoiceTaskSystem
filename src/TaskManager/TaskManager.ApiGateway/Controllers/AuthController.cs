using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Auth.Application.Features.Auth.ChangeMyPassword;
using TaskManager.Auth.Application.Features.Auth.ConfirmEmail;
using TaskManager.Auth.Application.Features.Auth.DeleteUser;
using TaskManager.Auth.Application.Features.Auth.GenerateTelegramLinkToken;
using TaskManager.Auth.Application.Features.Auth.GetMyProfile;
using TaskManager.Auth.Application.Features.Auth.SendEmailVerification;
using TaskManager.Auth.Application.Features.Auth.UnlinkTelegramChat;
using TaskManager.Auth.Application.Features.Auth.UpdateUserProfile;
using TaskManager.ApiGateway.DTOs.Auth;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Interfaces;

namespace TaskManager.ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController(IMediator mediator, ICurrentUser user) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _user = user;

    /// <summary>
    /// Получить профиль текущего пользователя
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(SuccessResponse<GetMyProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me()
    {
        var result = await _mediator.Send(new GetMyProfileQuery(_user.UserId));
        return Success(result);
    }

    /// <summary>
    /// Отправить email для подтверждения почты
    /// </summary>
    [HttpPost("email/send-verification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendVerification()
    {
        await _mediator.Send(new SendEmailVerificationCommand(_user.UserId));
        return Ok();
    }

    /// <summary>
    /// Подтвердить email по токену
    /// </summary>
    [HttpPost("email/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto request)
    {
        await _mediator.Send(new ConfirmEmailCommand(_user.UserId, request.Token));
        return Ok();
    }

    /// <summary>
    /// Получить токен для привязки Telegram
    /// </summary>
    [HttpGet("telegram-link-token")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTelegramLinkToken()
    {
        var result = await _mediator.Send(new GenerateTelegramLinkTokenCommand(_user.UserId));
        return Success(result);
    }

    /// <summary>
    /// Отвязать Telegram аккаунт
    /// </summary>
    [HttpDelete("telegram-link")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UnlinkTelegram()
    {
        await _mediator.Send(new UnlinkTelegramCommand(_user.UserId));
        return Success(new { });
    }

    /// <summary>
    /// Обновить профиль пользователя
    /// </summary>
    [HttpPatch("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto request)
    {
        var command = new UpdateUserProfileCommand(_user.UserId, request.Name, request.Email);
        await _mediator.Send(command);

        return Success(new { });
    }

    /// <summary>
    /// Изменить пароль текущего пользователя
    /// </summary>
    [HttpPatch("change-password")]
    [ProducesResponseType(typeof(SuccessResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangeMyPasswordDto request)
    {
        var command = new ChangeMyPasswordCommand(
            _user.UserId,
            request.CurrentPassword,
            request.NewPassword);

        var result = await _mediator.Send(command);

        return Success(new { isUpdated = result });
    }

    /// <summary>
    /// Удалить аккаунт пользователя
    /// </summary>
    [HttpDelete("account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAccount()
    {
        await _mediator.Send(new DeleteUserCommand(_user.UserId));

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
}