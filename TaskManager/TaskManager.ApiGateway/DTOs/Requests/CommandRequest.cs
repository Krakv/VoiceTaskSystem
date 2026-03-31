namespace TaskManager.Application.Common.DTOs.Requests;

public record CommandRequest(
    Guid CommandId,
    string CommandText
);
