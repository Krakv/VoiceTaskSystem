using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskManager.ApiGateway.DTOs.Rule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.DeleteRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.GetRules;
using TaskManager.RulesEngine.Application.Features.RuleFeature.ToggleRule;
using TaskManager.RulesEngine.Application.Features.RuleFeature.UpdateRule;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Interfaces;
using TaskManager.Shared.Utils;

namespace TaskManager.ApiGateway.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/rules")]
[Produces("application/json")]
public class RuleItemController(IMediator mediator, ICurrentUser user) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _user = user;

    /// <summary>
    /// Создать правило автоматизации (Rule Engine)
    /// </summary>
    /// <remarks>
    /// Создаёт правило, состоящее из:
    /// - события (RuleEvent)
    /// - условий (ConditionGroup JSON)
    /// - действий (RuleAction JSON array)
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(SuccessResponse<CreateRuleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRuleDto dto)
    {
        var ruleEvent = Enum.Parse<RuleEvent>(dto.RuleEvent, true);

        var conditions = dto.Conditions is null
            ? null
            : JsonSerializer.Deserialize<ConditionGroup>(
                dto.Conditions.ToString()!,
                JsonHelper.Default);

        var actions = dto.Actions
            .Select(a => JsonSerializer.Deserialize<RuleAction>(
                a.ToString()!,
                JsonHelper.Default)!)
            .ToList();

        var command = new CreateRuleCommand(
            _user.UserId,
            ruleEvent,
            conditions,
            actions,
            dto.IsActive
        );

        var result = await _mediator.Send(command);

        return CreatedResponse($"api/v1/rules/{result.RuleId}", result);
    }

    /// <summary>
    /// Получить все правила пользователя
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SuccessResponse<GetRulesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetRulesQuery(_user.UserId));
        return Success(result);
    }

    /// <summary>
    /// Получить правило по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<GetRulesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetRuleQuery(_user.UserId, id));
        return Success(result);
    }

    /// <summary>
    /// Обновить правило
    /// </summary>
    /// <remarks>
    /// Полностью перезаписывает rule definition:
    /// - RuleEvent
    /// - Conditions (JSON)
    /// - Actions (JSON array)
    /// - IsActive
    /// </remarks>
    [HttpPut("{ruleId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid ruleId, [FromBody] UpdateRuleDto dto)
    {
        var ruleEvent = Enum.Parse<RuleEvent>(dto.RuleEvent, true);

        var conditions = dto.Conditions is null
            ? null
            : JsonSerializer.Deserialize<ConditionGroup>(
                dto.Conditions.ToString()!,
                JsonHelper.Default);

        var actions = dto.Actions
            .Select(a => JsonSerializer.Deserialize<RuleAction>(
                a.ToString()!,
                JsonHelper.Default)!)
            .ToList();

        var command = new UpdateRuleCommand(
            _user.UserId,
            ruleId,
            ruleEvent,
            conditions,
            actions,
            dto.IsActive
        );

        await _mediator.Send(command);

        return Success(new { isUpdated = true });
    }

    /// <summary>
    /// Включить/выключить правило (toggle)
    /// </summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Toggle(Guid id)
    {
        await _mediator.Send(new ToggleRuleCommand(_user.UserId, id));
        return Success(new { isToggled = true });
    }

    /// <summary>
    /// Удалить правило
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteRuleCommand(_user.UserId, id));
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

    /// <summary>
    /// 201 Created ответ API
    /// </summary>
    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta
        {
            RequestId = HttpContext.TraceIdentifier
        }));
}