namespace TaskManager.ApiGateway.DTOs.Requests;

public record CommandRequest(
    Guid CommandId,
    string CommandText
);
