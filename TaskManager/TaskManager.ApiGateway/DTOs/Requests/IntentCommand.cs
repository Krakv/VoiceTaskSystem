namespace TaskManager.Application.Common.DTOs.Requests;

public record IntentCommand(Dictionary<string, string> commandParameters, long chatId);
