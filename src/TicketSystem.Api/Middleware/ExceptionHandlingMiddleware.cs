using System.Net;
using System.Text.Json;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Exceptions;

namespace TicketSystem.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var requestId = context.TraceIdentifier;
        var (statusCode, error) = exception switch
        {
            NotFoundException notFound => (HttpStatusCode.NotFound, new ApiError
            {
                Error = notFound.Message,
                RequestId = requestId
            }),
            InvalidTransitionException invalid => (HttpStatusCode.BadRequest, new ApiError
            {
                Error = "Invalid status transition",
                CurrentStatus = invalid.CurrentStatus,
                RequestedStatus = invalid.RequestedStatus,
                Message = invalid.Message,
                RequestId = requestId
            }),
            BusinessValidationException validation => (HttpStatusCode.BadRequest, new ApiError
            {
                Error = validation.Message,
                Errors = validation.Errors,
                RequestId = requestId
            }),
            _ => (HttpStatusCode.InternalServerError, new ApiError
            {
                Error = "An unexpected error occurred",
                RequestId = requestId
            })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception. RequestId: {RequestId}", requestId);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(error, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
