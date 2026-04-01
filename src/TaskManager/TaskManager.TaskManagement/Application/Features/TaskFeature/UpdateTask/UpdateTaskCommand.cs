using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;

public sealed record UpdateTaskCommand(
    string TaskId,
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    string DueDate,
    string? ParentTaskId
    ) : IRequest<string>;
