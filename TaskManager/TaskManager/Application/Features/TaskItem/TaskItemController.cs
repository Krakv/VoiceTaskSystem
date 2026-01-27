using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.Application.Features.CommandRequestItem.CreateVoiceTask;
using TaskManager.Application.Features.TaskItem.CreateTask;
using TaskManager.Application.Features.TaskItem.DeleteTask;
using TaskManager.Application.Features.TaskItem.DTOs;
using TaskManager.Application.Features.TaskItem.GetTask;
using TaskManager.Application.Features.TaskItem.GetTasks;
using TaskManager.Application.Features.TaskItem.UpdateTask;
using TaskManager.Application.Features.TaskItem.UpdateTaskPatch;

namespace TaskManager.Application.Features.TaskItem;

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
    public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery query)
    {
        var response = await _mediator.Send(query);

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
            dto.Tags,
            dto.ParentTaskId
            ));

        return Success(response);
    }

    [HttpPatch("{taskId}")]
    public async Task<IActionResult> UpdateTaskPatch([FromBody] UpdateTaskDto dto, string taskId)
    {
        var response = await _mediator.Send(new UpdateTaskPatchCommand(
            taskId,
            dto.ProjectName,
            dto.Title,
            dto.Description,
            dto.Status,
            dto.Priority,
            dto.DueDate,
            dto.Tags,
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

    private OkObjectResult Success<T>(T data) where T : class =>
        Ok(new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

    private CreatedResult CreatedResponse<T>(string location, T data) where T : class =>
        Created(location, new SuccessResponse<T>(data, new Meta { RequestId = HttpContext.TraceIdentifier }));

}
