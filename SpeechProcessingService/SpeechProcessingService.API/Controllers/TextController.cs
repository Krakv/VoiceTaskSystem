using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpeechProcessingService.Application.Features.Text.ClassifyIntent;
using SpeechProcessingService.Application.Features.Text.ExtractEntities;

namespace SpeechProcessingService.Application.Features.Text;

[Route("api/text/[controller]")]
[ApiController]
public class TextController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("classify-intent")]
    public async Task<IActionResult> ClassifyIntent([FromForm] ClassifyIntentCommand command)
    {
        if (command == null)
            return BadRequest("Text is missing.");

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpPost("extract-entities")]
    public async Task<IActionResult> ExtractEntities([FromForm] ExtractEntitiesCommand command)
    {
        if (command == null)
            return BadRequest("Text is missing.");

        var response = await _mediator.Send(command);

        return Ok(response);
    }
}
