namespace TaskManager.ApiGateway.DTOs.Responses;

public class ErrorResponse : ApiResponseBase
{
    public Error Error { get; set; }

    public ErrorResponse(Error error, Meta meta)
    {
        Success = false;
        Error = error;
        Meta = meta;
    }
}
