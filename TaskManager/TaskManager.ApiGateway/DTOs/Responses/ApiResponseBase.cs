namespace TaskManager.Application.Common.DTOs.Responses;

public abstract class ApiResponseBase 
{
    public bool Success { get; set; }
    public Meta Meta { get; set; } = default!;
}
