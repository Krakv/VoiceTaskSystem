using System.Collections.Frozen;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.RulesEngine.Application.Services;

public static class TaskFieldMap
{
    public static readonly FrozenSet<string> Allowed =
        new[]
        {
            nameof(TaskItem.Title),
            nameof(TaskItem.Priority),
            nameof(TaskItem.Status),
            nameof(TaskItem.ProjectName),
            nameof(TaskItem.DueDate),
            nameof(TaskItem.Description),
            nameof(TaskItem.ParentTaskId)
        }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
}
