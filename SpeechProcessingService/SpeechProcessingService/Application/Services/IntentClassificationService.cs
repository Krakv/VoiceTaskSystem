using System;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Services;

public class IntentClassificationService(IGenAIService genService) : IIntentClassificationService
{
    private readonly IGenAIService _genService = genService;

    public string ClassifyIntent(string commandText)
    {
        var prompt = $"""
            Необходимо классифицировать предложение 
            Есть 7 классов:
            update_task_priority = обновить приоритет задачи, 
            update_task_project = обновить проект задачи, 
            create_task_reminder = создать напоминание для задачи, 
            delete_task = удалить задачу, 
            create_task = создать задачу, 
            update_task_due_date = обновить срок задачи, 
            list_tasks = вывести список задач
            unknown = неизвестная задача

            Твой ответ должен состоять только из названия класса, которому относится предложение (без скобок и кавычек):
            "{commandText}"
            """;

        return _genService.GetAnswer(prompt).GetAwaiter().GetResult();
    }
}
