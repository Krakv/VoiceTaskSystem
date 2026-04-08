using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.ApiGateway.DTOs;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.DeleteRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;
using TaskManager.RulesEngine.Application.Features.RuleFeature.ToggleRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.UpdateRule;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.ApiGateway.Controllers;

[ApiController]
[Route("api/v1/rules")]
public class RuleItemController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRuleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedResponse($"api/v1/rules/{result.RuleId}", result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetRulesQuery());
        return Success(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _mediator.Send(new GetRuleQuery(id));
        return Success(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateRuleDto dto)
    {
        var updateRuleCommand = new UpdateRuleCommand
        (
            id,
            dto.RuleEvent,
            dto.Conditions,
            dto.Actions,
            dto.IsActive
        );

        await _mediator.Send(updateRuleCommand);
        return Success(new {});
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Toggle(string id)
    {
        await _mediator.Send(new ToggleRuleCommand(id));
        return Success(new { });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new DeleteRuleCommand(id));
        return Success(new { });
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}
