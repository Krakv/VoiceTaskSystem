namespace TaskManager.ApiGateway.DTOs.CommandRequest;

public sealed record UpdateVoiceTaskDto(
    string? TaskId,
    string? ProjectName,
    string? Title,
    string? Description,
    string? Status,
    string? DueDate,
    string? Priority,
    string? ParentTaskId
    );