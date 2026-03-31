namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.DTOs;

public sealed record UpdateVoiceTaskDto(
    string? ProjectName,
    string? Title,
    string? Description,
    string? Status,
    string? DueDate,
    string? Priority
    );