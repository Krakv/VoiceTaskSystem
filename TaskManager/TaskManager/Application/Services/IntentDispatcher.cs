using System.Text.Encodings.Web;
using System.Text.Json;
using TaskManager.Application.Common.DTOs.Requests;
using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.Application.Domain.Entities;
using TaskManager.Application.Services.Interfaces;

namespace TaskManager.Application.Services;

public class IntentDispatcher() : IIntentDispatcher
{

    public Task<IntentResult> DispatchAsync(IntentCommand command)
    {
        throw new NotImplementedException();
    }

    public static string ToReadableString(TaskItem task)
    {
        // Приоритет по умолчанию
        var priority = string.IsNullOrWhiteSpace(task.Priority) ? "none" : task.Priority;

        // Проект
        var project = string.IsNullOrWhiteSpace(task.ProjectName) ? "-" : task.ProjectName;

        // Заголовок
        var title = string.IsNullOrWhiteSpace(task.Title) ? "(без названия)" : task.Title;

        // Описание
        var description = string.IsNullOrWhiteSpace(task.Description) ? "-" : task.Description;

        // Срок
        var dueDate = task.DueDate == null
            ? "не указан"
            : task.DueDate.Value.ToString("dd.MM.yyyy HH:mm");

        // Статус
        var status = string.IsNullOrWhiteSpace(task.Status) ? "-" : task.Status;

        return $"""
            [{priority}] {title}
            Проект: {project}
            Описание: {description}
            Срок: {dueDate}
            Статус: {status}
            """;
    }

    public static string ToReadableList(IEnumerable<TaskItem> tasks)
    {
        if (!tasks.Any())
            return "Список задач пуст";

        return "Текущие задачи:\n" + string.Join("\n\n", tasks.Select(ToReadableString));
    }

}
