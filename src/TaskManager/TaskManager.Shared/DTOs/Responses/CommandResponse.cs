using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Shared.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    string TextRecognized,
    CommandIntent CommandIntent,
    Dictionary<string, string> Entities
  );
