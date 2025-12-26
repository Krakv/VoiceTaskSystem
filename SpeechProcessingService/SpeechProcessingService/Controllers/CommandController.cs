using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeechProcessingService.Application.DTOs.Requests;
using SpeechProcessingService.Application.DTOs.Responses;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Controllers;
    

[Route("api/speech/[controller]")]
[ApiController]
public class CommandController(ISpeechProcessingService speechProcesser) : ControllerBase
{
    private readonly ISpeechProcessingService _speechProcesser = speechProcesser;

    [HttpPost("process")]
    public IActionResult ProcessCommand([FromBody] CommandRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.CommandText))
        {
            return BadRequest("Invalid command request.");
        }

        // Сommand processing logic
        var processedCommand = _speechProcesser.ProcessCommand(request.CommandText);

        return Ok(processedCommand);
    }
}
