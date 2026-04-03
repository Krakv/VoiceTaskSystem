using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpeechProcessingService.API.DTOs;
using SpeechProcessingService.Application.DTOs;
using SpeechProcessingService.Application.Features.Audio.ProcessAudio;
using SpeechProcessingService.Application.Features.Audio.RecognizeSpeech;

namespace SpeechProcessingService.API.Controllers;

[Route("api/speech/audio")]
[ApiController]
public class AudioController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("process")]
    public async Task<IActionResult> ProcessAudio([FromForm] ProcessAudioDto dto)
    {
        var formFile = dto.FormFile;

        if (formFile == null || formFile.Length == 0)
            return BadRequest("Audio file is missing.");

        byte[] content;
        using (var memoryStream = new MemoryStream())
        {
            await formFile.CopyToAsync(memoryStream);
            content = memoryStream.ToArray();
        }

        var command = new ProcessAudioCommand(
            dto.CommandRequestId,
            new AudioFile(
                formFile.FileName,
                formFile.ContentType,
                content
            ),
            TimeZoneInfo.FindSystemTimeZoneById(dto.UserTimeZone)
        );

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpPost("recognize")]
    public async Task<IActionResult> Recognize([FromForm] FormFileDto formFileDto)
    {
        var formFile = formFileDto.FormFile;

        if (formFile == null || formFile.Length == 0)
            return BadRequest("Audio file is missing.");

        byte[] content;
        using (var memoryStream = new MemoryStream())
        {
            await formFile.CopyToAsync(memoryStream);
            content = memoryStream.ToArray();
        }

        var command = new RecognizeSpeechCommand(
            new AudioFile(
                formFile.FileName,
                formFile.ContentType,
                content
            )
        );

        var response = await _mediator.Send(command);

        return Ok(new { text = response });
    }
}