using System.Globalization;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Shared.Domain.Builders;

public class TaskItemBuilder
{
    private readonly TaskItem _task;

    public TaskItemBuilder(Guid ownerId)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("OwnerId is required");

        _task = new TaskItem
        {
            TaskId = Guid.NewGuid(),
            OwnerId = ownerId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = TaskItemStatus.New
        };
    }

    public TaskItemBuilder SetProject(string? projectName)
    {
        _task.ProjectName = projectName ?? string.Empty;
        return this;
    }

    public TaskItemBuilder SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");
        _task.Title = title;
        return this;
    }

    public TaskItemBuilder SetDescription(string? description)
    {
        _task.Description = description ?? string.Empty;
        return this;
    }

    public TaskItemBuilder SetStatus(TaskItemStatus? status)
    {
        _task.Status = status ?? TaskItemStatus.New;
        return this;
    }

    public TaskItemBuilder SetPriority(TaskItemPriority? priority)
    {
        _task.Priority = priority ?? TaskItemPriority.Low;
        return this;
    }

    public TaskItemBuilder SetDueDate(DateTimeOffset? dueDate)
    {
        _task.DueDate = dueDate;
        return this;
    }

    public TaskItemBuilder SetParent(Guid? parentTaskId)
    {
        _task.ParentTaskId = parentTaskId;
        return this;
    }

    public TaskItem Build()
    {
        return _task;
    }
}