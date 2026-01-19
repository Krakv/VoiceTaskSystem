using MediatR;

namespace TaskManager.Application.Features.TaskItem.UpdateTask;

public sealed record UpdateTaskCommand(
    string TaskId,
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    string DueDate
    ) : IRequest<UpdateTaskResponse>;
