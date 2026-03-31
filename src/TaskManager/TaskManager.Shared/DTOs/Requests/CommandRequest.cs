namespace TaskManager.Shared.DTOs.Requests;

public record CommandRequest(
    Guid CommandId,
    string CommandText
);
