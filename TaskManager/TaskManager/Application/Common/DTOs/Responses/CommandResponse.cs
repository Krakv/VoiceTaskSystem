namespace TaskManager.Application.Common.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    Dictionary<string, string> Parameters
  );
