using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

public sealed record CreateTaskCommand(
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    string DueDate,
    string ParentTaskId
    ) : IRequest<string>;
