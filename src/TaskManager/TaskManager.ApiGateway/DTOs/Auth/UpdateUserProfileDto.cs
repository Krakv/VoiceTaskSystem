namespace TaskManager.ApiGateway.DTOs.Auth;

public record UpdateUserProfileDto(
    string? Name,
    string? Email
);
