namespace TaskManager.ApiGateway.DTOs.Requests;

public record IntentCommand(Dictionary<string, string> commandParameters, long chatId);
