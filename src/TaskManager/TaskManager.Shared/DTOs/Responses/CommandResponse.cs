using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Models;

namespace TaskManager.Shared.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    string TextRecognized,
    CommandIntent CommandIntent,
    TaskItemModel Entities
  );
