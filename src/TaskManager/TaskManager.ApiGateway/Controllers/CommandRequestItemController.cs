using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.ApiGateway.DTOs;
using TaskManager.ApiGateway.DTOs.CommandRequest;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.ConfirmVoiceTask;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.CreateVoiceTask;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DeleteVoiceTask;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.UpdateVoiceTask;
using InputFile = TaskManager.Shared.DTOs.Requests.InputFile;

namespace TaskManager.ApiGateway.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/tasks/voice")]
[Produces("application/json")]
public class CommandRequestItemController(IMediator mediator, ICurrentUser user) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _user = user;

    /// <summary>
    /// Создать голосовую задачу (загрузка аудиофайла)
    /// </summary>
    /// <remarks>
    /// Принимает аудиофайл через multipart/form-data, отправляет его на обработку
    /// и создаёт асинхронный command request.
    /// </remarks>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SuccessResponse<CreateVoiceTaskResponse>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVoiceTask([FromForm] FormFileDto dto)
    {
        if (dto.FormFile == null)
        {
            return BadRequest(new ErrorResponse
            (
                error: new Error
                {
                    Code = "INVALID_AUDIO_FILE",
                    Message = "Файл отсутствует или неверный формат"
                },
                meta: new Meta { RequestId = HttpContext.TraceIdentifier }
            ));
        }

        byte[] content;

        using (var ms = new MemoryStream())
        {
            await dto.FormFile.CopyToAsync(ms);
            content = ms.ToArray();
        }

        var response = await _mediator.Send(new CreateVoiceTaskCommand(
            _user.UserId,
            new InputFile(
                fileName: dto.FormFile.FileName,
                contentType: dto.FormFile.ContentType,
                length: dto.FormFile.Length,
                content: content
            )
        ));

        return AcceptedResponse<CreateVoiceTaskResponse>(response);
    }

    /// <summary>
    /// Получить статус обработки голосовой задачи
    /// </summary>
    [HttpGet("requests/{commandRequestId:guid}/status")]
    [ProducesResponseType(typeof(SuccessResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVoiceTaskStatus(Guid commandRequestId)
    {
        var response = await _mediator.Send(
            new GetVoiceTaskStatusQuery(_user.UserId, commandRequestId));

        return Success(response);
    }

    /// <summary>
    /// Обновить результат распознанной голосовой задачи
    /// </summary>
    [HttpPatch("requests/{commandRequestId:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<UpdateVoiceTaskResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateVoiceTask(
        [FromBody] UpdateVoiceTaskDto dto,
        Guid commandRequestId)
    {
        var response = await _mediator.Send(new UpdateVoiceTaskCommand(
            _user.UserId,
            commandRequestId,
            dto.TaskId == null ? null : Guid.Parse(dto.TaskId),
            dto.ProjectName,
            dto.Title,
            dto.Description,
            dto.Status == null ? null : Enum.Parse<TaskItemStatus>(dto.Status),
            dto.DueDate,
            dto.Priority == null ? null : Enum.Parse<TaskItemPriority>(dto.Priority),
            dto.ParentTaskId
        ));

        return Success(response);
    }

    /// <summary>
    /// Подтвердить голосовую задачу (создание/применение результата)
    /// </summary>
    [HttpPost("requests/{commandRequestId:guid}/confirm")]
    [ProducesResponseType(typeof(SuccessResponse<ConfirmVoiceTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> ConfirmVoiceTask(Guid commandRequestId)
    {
        var response = await _mediator.Send(
            new ConfirmVoiceTaskCommand(_user.UserId, commandRequestId));

        if (response.Intent == "TASK_CREATE")
        {
            return CreatedResponse<ConfirmVoiceTaskResponse>(
                $"api/tasks/{response.TaskId}",
                response);
        }

        return Success(response);
    }

    /// <summary>
    /// Удалить голосовую задачу (command request)
    /// </summary>
    [HttpDelete("requests/{commandRequestId:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<DeleteVoiceTaskResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteVoiceTask(Guid commandRequestId)
    {
        var response = await _mediator.Send(
            new DeleteVoiceTaskCommand(_user.UserId, commandRequestId));

        return Success(response);
    }

    /// <summary>
    /// 200 OK успешный ответ
    /// </summary>
    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));

    /// <summary>
    /// 202 Accepted (асинхронная обработка)
    /// </summary>
    private AcceptedResult AcceptedResponse<T>(T data) where T : class =>
        Accepted(new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));

    /// <summary>
    /// 201 Created
    /// </summary>
    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));
}