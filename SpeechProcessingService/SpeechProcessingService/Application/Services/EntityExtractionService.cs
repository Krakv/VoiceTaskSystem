using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Services;

public class EntityExtractionService(IGenAIService genService) : IEntityExtractionService
{
    private readonly IGenAIService _genService = genService;

    public Dictionary<string, string> ExtractEntities(string command, string intent)
    {
        var prompt = $"""
            Необходимо выделить в предложении следующие сущности (Если они есть):
            name (Название задачи)
            project_name (Название проекта, к которому относится задача)
            priority (Приоритет задачи high, medium, low)
            due_date (Дедлайн)
            description (Описание задачи)

            Твой ответ должен содержать только сообщение в следующем виде (Если о сущности нет информации, то просто пустое место):
            name::Название;project_name::название проекта;priority::high;due_date::2024-01-15T14:30:25;description::Описание

            
            Текущая дата и время: {DateTime.UtcNow}
            Вот сообщение, которое надо разробрать на сущности: "{command}"
            """;

        return HandleAnswer(prompt);
    }

    public Dictionary<string, string> HandleAnswer(string prompt)
    {
        var answer = _genService.GetAnswer(prompt).GetAwaiter().GetResult();
        Dictionary<string, string> res = new();
        foreach (var item in answer.Split(";"))
        {
            var splittedItem = item.Split("::");
            if (splittedItem.Length > 1)
                res.Add(splittedItem[0], splittedItem[1]);
        }
        return res;
    }
}
