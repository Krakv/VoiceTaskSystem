namespace TaskManager.Shared.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    Dictionary<string, string> Parameters
  );
