namespace TaskManager.ApiGateway.DTOs;

public sealed record UpdateVoiceTaskDto(
    string? ProjectName,
    string? Title,
    string? Description,
    string? Status,
    string? DueDate,
    string? Priority
    );