using System.Text.Encodings.Web;
using System.Text.Json;
using TaskManager.Application.Common.DTOs.Requests;
using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.Application.Features.TaskItem;
using TaskManager.Application.Services.Interfaces;

namespace TaskManager.Application.Services;

public class IntentDispatcher() : IIntentDispatcher
{

    public async Task<IntentResult> DispatchAsync(IntentCommand command)
    {
        throw new NotImplementedException();

        //var intent = command.commandParameters["intent"];

        //switch (intent)
        //{
        //    case "create_task":
        //        {
        //            var crtTaskCmd = new CreateTaskCommand(command.commandParameters, command.chatId);
        //            var taskItem = await _createTaskHandler.CreateTask(crtTaskCmd);
        //            return new IntentResult("Создана задача: " + ToReadableString(taskItem));
        //        }
        //    case "delete_task":
        //        {
        //            var dltTaskCmd = new DeleteTaskCommand(command.commandParameters["name"], command.chatId);
        //            var isDeleted = await _deleteTaskHandler.DeleteTask(dltTaskCmd);
        //            return new IntentResult(isDeleted ? "Задача удалена" : "Задача не найдена");
        //        }
        //    case "list_tasks":
        //        {
        //            var rdTaskCmd = new ReadTaskListCommand(command.chatId);
        //            var taskItems = await _readTaskListHandler.ReadTaskList(rdTaskCmd);
        //            return new IntentResult(ToReadableList(taskItems));
        //        }
        //    default:
        //        return new IntentResult("Неизвестная команда");
        //}

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
