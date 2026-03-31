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
        throw new NotImplementedException();
    }

    public static string ToReadableList(IEnumerable<TaskItem> tasks)
    {
        if (!tasks.Any())
            return "Список задач пуст";

        return "Текущие задачи:\n" + string.Join("\n\n", tasks.Select(ToReadableString));
    }

}
