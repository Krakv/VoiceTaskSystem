namespace TaskManager.Shared.DTOs.Requests;

public record IntentCommand(Dictionary<string, string> commandParameters, long chatId);
