using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.Exceptions;

namespace TaskManager.Middleware;

public class HttpErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpErrorHandlingMiddleware> _logger;

    public HttpErrorHandlingMiddleware(RequestDelegate next, ILogger<HttpErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext? context)
    {
        if (context == null)
        {
            await _next(null!);
            return;
        }

        try
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";

                var errorResponse = new ErrorResponse(
                    error: new Error
                    {
                        Code = "UNAUTHORIZED",
                        Message = "Доступ запрещен"
                    },
                    meta: new Meta { RequestId = context.TraceIdentifier }
                );

                // Сброс текущего потока ответа
                context.Response.Body.SetLength(0);
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
        catch (ValidationAppException ex)
        {
            _logger.LogInformation(ex, "Validation exception in HTTP request");

            var statusCode = ex.ErrorCode switch
            {
                "INVALID_PARAMS" => StatusCodes.Status400BadRequest,
                "INVALID_AUDIO_FILE" => StatusCodes.Status400BadRequest,
                "UNSUPPORTED_MEDIA_TYPE" => StatusCodes.Status415UnsupportedMediaType,
                "NOT_FOUND" => StatusCodes.Status404NotFound,
                "PENDING" => StatusCodes.Status202Accepted,
                "FORBIDDEN" => StatusCodes.Status403Forbidden,
                "ALREADY_CONFIRMED" => StatusCodes.Status409Conflict,
                "CANCELLED" => StatusCodes.Status409Conflict,
                "ALREADY_PROCESSED" => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse(
                error: new Error
                {
                    Code = ex.ErrorCode,
                    Message = ex.Message,
                },
                meta: new Meta { RequestId = context.TraceIdentifier }
            );

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in HTTP request");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse(
                error: new Error
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "Внутренняя ошибка сервера"
                },
                meta: new Meta { RequestId = context.TraceIdentifier }
            );

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}

