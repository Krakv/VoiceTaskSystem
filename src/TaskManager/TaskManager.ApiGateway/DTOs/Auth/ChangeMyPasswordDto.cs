namespace TaskManager.ApiGateway.DTOs.Auth;

public record ChangeMyPasswordDto(
    string CurrentPassword,
    string NewPassword
);
