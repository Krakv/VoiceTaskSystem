using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TaskManager.ApiGateway.DTOs.Task;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetProjects;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

namespace TaskManager.ApiGateway.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/tasks")]
public class TaskItemController(IMediator mediator, ICurrentUser user) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _user = user;

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        var command = new CreateTaskCommand(
            _user.UserId,
            dto.ProjectName,
            dto.Title,
            dto.Description,
            dto.Status,
            dto.Priority,
            DateTimeOffset.TryParse(dto.DueDate, CultureInfo.InvariantCulture, out var dueDate) ? dueDate : null,
            string.IsNullOrWhiteSpace(dto.ParentTaskId) ? null : Guid.Parse(dto.ParentTaskId)
        );

        var response = await _mediator.Send(command);

        return CreatedResponse($"api/v1/tasks/{response}", response.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] GetTasksDto dto)
    {
        _ = int.TryParse(dto.Limit, out var limit);
        _ = int.TryParse(dto.Page, out var page);

        var query = new GetTasksQuery(
            _user.UserId,
            dto.Query,
            dto.Status,
            dto.Priority,
            dto.SortBy,
            dto.SortOrder,
            limit,
            page
        );

        var response = await _mediator.Send(query);

        return Success(response);
    }

    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetTask(Guid taskId)
    {
        var response = await _mediator.Send(new GetTaskQuery(_user.UserId, taskId));

        return Success(response);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskDto dto)
    {
        var command = new UpdateTaskCommand(
            _user.UserId,
            taskId,
            dto.ProjectName,
            dto.Title,
            dto.Description,
            dto.Status,
            dto.Priority,
            DateTimeOffset.TryParse(dto.DueDate, CultureInfo.InvariantCulture, out var dueDate) ? dueDate : null,
            string.IsNullOrWhiteSpace(dto.ParentTaskId) ? null : Guid.Parse(dto.ParentTaskId)
        );

        var response = await _mediator.Send(command);

        return Success(response.ToString());
    }

    [HttpPatch("{taskId:guid}")]
    public async Task<IActionResult> UpdateTaskPatch(Guid taskId, [FromBody] UpdateTaskPatchDto dto)
    {
        if (dto is null)
            return BadRequest("Body cannot be null");

        var command = new UpdateTaskPatchCommand(
            _user.UserId,
            taskId,
            dto.ProjectName,
            dto.Title,
            dto.Description,
            Enum.TryParse(dto.Status, true, out TaskItemStatus status) ? status : null,
            Enum.TryParse(dto.Priority, true, out TaskItemPriority priority) ? priority : null,
            dto.DueDate,
            dto.ParentTaskId
        );

        var response = await _mediator.Send(command);

        return Success(response.ToString());
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var response = await _mediator.Send(new DeleteTaskCommand(_user.UserId, taskId));

        return Success(response.ToString());
    }

    [HttpGet("projects")]
    public async Task<IActionResult> GetProjectNames([FromQuery] GetProjectsDto dto)
    {
        _ = int.TryParse(dto.Page, out int page);
        _ = int.TryParse(dto.Limit, out int limit);

        var query = new GetProjectsCommand(
            _user.UserId,
            dto.Project,
            page,
            limit
        );

        var response = await _mediator.Send(query);

        return Success(response);
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));
}