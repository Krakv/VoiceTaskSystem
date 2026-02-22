using System.Globalization;
using TaskManager.Application.Domain.Entities;

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
            Status = "Pending"
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
        if (!string.IsNullOrWhiteSpace(status))
            _task.Status = status;

        return this;
    }

    public TaskItemBuilder SetPriority(string? priority)
    {
        _task.Priority = priority ?? string.Empty;
        return this;
    }

    public TaskItemBuilder SetTags(string? tags)
    {
        _task.Tags = tags ?? string.Empty;
        return this;
    }

    public TaskItemBuilder SetDueDate(string? dueDate)
    {
        if (!string.IsNullOrWhiteSpace(dueDate))
        {
            _task.DueDate = DateTimeOffset.Parse(dueDate, CultureInfo.InvariantCulture);
        }

        return this;
    }

    public TaskItemBuilder SetParent(string? parentTaskId)
    {
        if (!string.IsNullOrWhiteSpace(parentTaskId))
        {
            _task.ParentTaskId = Guid.Parse(parentTaskId);
        }

        return this;
    }

    public TaskItem Build()
    {
        return _task;
    }
}
