namespace TaskManager.Shared.DTOs.Responses;

public class SuccessResponse<T> : ApiResponseBase where T : class
{
    public T Data { get; set; }

    public SuccessResponse(T data, Meta meta)
    {
        Success = true;
        Data = data;
        Meta = meta;
    }
}
