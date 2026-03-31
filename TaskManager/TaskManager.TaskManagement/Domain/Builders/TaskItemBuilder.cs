using System.Globalization;
using TaskManager.Application.Domain.Entities;
using TaskManager.Application.Domain.Entities.Enum;
namespace TaskManager.Application.Domain.Builders;

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

    public TaskItemBuilder SetStatus(string? status)
    {
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<TaskItemStatus>(status, ignoreCase: true, out var parsed))
        {
            _task.Status = parsed;
        }
        return this;
    }

    public TaskItemBuilder SetPriority(string? priority)
    {
        if (!string.IsNullOrWhiteSpace(priority) &&
            Enum.TryParse<TaskItemPriority>(priority, ignoreCase: true, out var parsed))
        {
            _task.Priority = parsed;
        }
        return this;
    }

    public TaskItemBuilder SetDueDate(string? dueDate)
    {
        if (!string.IsNullOrWhiteSpace(dueDate))
            _task.DueDate = DateTimeOffset.Parse(dueDate, CultureInfo.InvariantCulture);
        return this;
    }

    public TaskItemBuilder SetParent(string? parentTaskId)
    {
        if (!string.IsNullOrWhiteSpace(parentTaskId))
            _task.ParentTaskId = Guid.Parse(parentTaskId);
        return this;
    }

    public TaskItem Build()
    {
        return _task;
    }
}