using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.Application.Features.CommandRequestItem.ConfirmVoiceTask;
using TaskManager.Application.Features.CommandRequestItem.CreateVoiceTask;
using TaskManager.Application.Features.CommandRequestItem.DeleteVoiceTask;
using TaskManager.Application.Features.CommandRequestItem.DTOs;
using TaskManager.Application.Features.CommandRequestItem.GetVoiceTaskStatus;
using TaskManager.Application.Features.CommandRequestItem.UpdateVoiceTask;
using Telegram.Bot.Types;
using InputFile = TaskManager.Application.Features.CommandRequestItem.CreateVoiceTask.InputFile;

namespace TaskManager.Application.Features.CommandRequestItem;

[ApiController]
[Authorize]
[Route("api/v1/tasks/voice")]
public class CommandRequestItemController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateVoiceTask([FromForm] IFormFile audioFile)
    {
        if (audioFile == null)
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

        using var stream = audioFile.OpenReadStream();

        var response = await _mediator.Send(new CreateVoiceTaskCommand(
            new InputFile(
                fileName: audioFile.FileName,
                contentType: audioFile.ContentType,
                length: audioFile.Length,
                content: stream
            )
        ));

        return AcceptedResponse<CreateVoiceTaskResponse>(response);
    }

    [HttpGet("requests/{commandRequestId}/status")]
    public async Task<IActionResult> GetVoiceTaskStatus(string commandRequestId)
    {
        var response = await _mediator.Send(new GetVoiceTaskStatusQuery(commandRequestId));

        return Success(response);
    }

    [HttpPatch("requests/{commandRequestId}")]
    public async Task<IActionResult> UpdateVoiceTask([FromBody] UpdateVoiceTaskDto dto, string commandRequestId)
    {
        var response = await _mediator.Send(new UpdateVoiceTaskCommand(commandRequestId, dto.ProjectName, dto.Title, dto.Description, dto.Status, dto.DueDate, dto.Priority));

        return Success(response);
    }

    [HttpPost("requests/{commandRequestId}")]
    public async Task<IActionResult> ConfirmVoiceTask(string commandRequestId)
    {
        var response = await _mediator.Send(new ConfirmVoiceTaskCommand(commandRequestId));

        if (response.Intent == "TASK_CREATE") {
            return CreatedResponse<ConfirmVoiceTaskResponse>($"api/tasks/{response.TaskId}", response);
        }

        return Success(response);
    }

    [HttpDelete("requests/{commandRequestId}")]
    public async Task<IActionResult> DeleteVoiceTask(string commandRequestId)
    {
        var response = await _mediator.Send(new DeleteVoiceTaskCommand(commandRequestId));

        return Success(response);
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private AcceptedResult AcceptedResponse<T>(T data) where T : class =>
        Accepted(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}
