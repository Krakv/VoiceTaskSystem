using MediatR;

namespace TaskManager.Application.Features.TaskItem.CreateTask;

public sealed record CreateTaskCommand(
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    string DueDate
    ) : IRequest<CreateTaskResponse>;
