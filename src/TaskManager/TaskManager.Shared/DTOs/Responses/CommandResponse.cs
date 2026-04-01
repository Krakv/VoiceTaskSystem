namespace TaskManager.Shared.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    string TextRecognized,
    Dictionary<string, string> Parameters
  );
