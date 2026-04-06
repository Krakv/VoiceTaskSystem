using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.DTOs;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;
using TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetProjects;

namespace TaskManager.ApiGateway.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/tasks")]
public class TaskItemController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
    {
        var response = await _mediator.Send(command);

        return CreatedResponse($"api/tasks/{response}", response);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(
            string? query,
            string? status,
            string? priority,
            string? sortBy,
            string? sortOrder,
            string? limit = "20",
            string? page = "0"
        )
    {
        var request = new GetTasksQuery(
            query,
            status,
            priority,
            sortBy,
            sortOrder,
            limit,
            page
        );

        var response = await _mediator.Send(request);

        return Success(response);
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTask(string taskId)
    {
        var response = await _mediator.Send(new GetTaskQuery(taskId));

        return Success(response);
    }

    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto dto, string taskId)
    {
        var response = await _mediator.Send(new UpdateTaskCommand(
            taskId,
            dto.ProjectName,
            dto.Title,
            dto.Description,
            dto.Status,
            dto.Priority,
            dto.DueDate,
            dto.ParentTaskId
            ));

        return Success(response);
    }

    [HttpPatch("{taskId}")]
    public async Task<IActionResult> UpdateTaskPatch([FromBody] UpdateTaskPatchDto dto, string taskId)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        var response = await _mediator.Send(new UpdateTaskPatchCommand(
            taskId,
            dto.ProjectName,
            dto.Title,
            dto.Description,
            dto.Status,
            dto.Priority,
            dto.DueDate,
            dto.ParentTaskId
            ));

        return Success(response);
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(string taskId)
    {
        var response = await _mediator.Send(new DeleteTaskCommand(taskId));

        return Success(response);
    }

    [HttpGet("projects")]
    public async Task<IActionResult> GetProjectNames([FromQuery] string projectName, [FromQuery] int page = 0, [FromQuery] int pageSize = 5)
    {
        var response = await _mediator.Send(new GetProjectsCommand(projectName, page, pageSize));

        return Success(response);
    }

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

}
