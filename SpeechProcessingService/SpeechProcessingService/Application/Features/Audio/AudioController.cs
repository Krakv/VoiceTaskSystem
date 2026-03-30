using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpeechProcessingService.Application.Features.Audio.ProcessAudio;
using SpeechProcessingService.Application.Features.Audio.RecognizeSpeech;

namespace SpeechProcessingService.Application.Features.Audio;

[Route("api/speech/[controller]")]
[ApiController]
public class AudioController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("process-audio")]
    public async Task<IActionResult> ProcessAudio([FromForm] ProcessAudioCommand command)
    {
        if (command == null || command.AudioFile == null || command.AudioFile.Length == 0)
            return BadRequest("Audio file is missing.");

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpPost("recognize")]
    public async Task<IActionResult> Recognize([FromForm] RecognizeSpeechCommand command)
    {
        if (command == null || command.AudioFile == null || command.AudioFile.Length == 0)
            return BadRequest("Audio file is missing.");

        var response = await _mediator.Send(command);

        return Ok(new { text = response });
    }
}