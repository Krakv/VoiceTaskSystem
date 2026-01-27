using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json;
using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.Exceptions;
using Telegram.Bot.Types;

namespace TaskManager.Middleware;

public class HttpErrorHandlingMiddleware(RequestDelegate next, ILogger<HttpErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<HttpErrorHandlingMiddleware> _logger = logger;

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

            if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600)
            {
                context.Response.ContentType = "application/json";

                var code = context.Response.StatusCode switch
                {
                    404 => "NOT_FOUND",
                    415 => "UNSUPPORTED_MEDIA_TYPE",
                    401 => "UNAUTHORIZED",
                    400 => "BAD_REQUEST",
                    403 => "FORBIDDEN",
                    409 => "CONFLICT",
                    202 => "PENDING",
                    405 => "METHOD_NOT_ALLOWED",
                    _ => "INTERNAL_SERVER_ERROR"
                };

                var message = context.Response.StatusCode switch
                {
                    404 => "Ресурс не найден",
                    415 => "Неподдерживаемый тип содержимого",
                    401 => "Токен доступа истёк или невалиден",
                    400 => "Неверный запрос",
                    403 => "Доступ запрещен",
                    409 => "Конфликт",
                    202 => "Операция в ожидании",
                    405 => "Вы обратились к правильному адресу, но использовали неподдерживаемый HTTP-метод",
                    _ => "Внутренняя ошибка сервера"
                };

                var errorResponse = new ErrorResponse(
                    error: new Error { Code = code, Message = message },
                    meta: new Meta { RequestId = context.TraceIdentifier }
                );

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
                "UNAUTHORIZED" => StatusCodes.Status401Unauthorized,
                "REGISTRATION_FAILED" => StatusCodes.Status400BadRequest,
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