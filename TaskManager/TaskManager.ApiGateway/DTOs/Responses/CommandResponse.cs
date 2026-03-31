namespace TaskManager.ApiGateway.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    Dictionary<string, string> Parameters
  );
