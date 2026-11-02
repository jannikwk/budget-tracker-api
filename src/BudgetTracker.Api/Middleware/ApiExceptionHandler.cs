using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Api.Middleware;

internal sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger): IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        return exception switch
        {
            KeyNotFoundException notFoundException =>
                HandleNotFoundAsync(httpContext, notFoundException, cancellationToken),
            ArgumentException argumentException =>
                HandleBadRequestAsync(httpContext, argumentException, cancellationToken),
            InvalidOperationException conflictException =>
                HandleConflictAsync(httpContext, conflictException, cancellationToken),
            _ => HandleUnexpectedAsync(httpContext, exception, cancellationToken)
        };
    }

    private ValueTask<bool> HandleNotFoundAsync(
        HttpContext httpContext,
        KeyNotFoundException exception,
        CancellationToken cancellationToken)
    {
        return HandleAsync(
            httpContext,
            exception,
            StatusCodes.Status404NotFound,
            "Not Found",
            LogLevel.Information,
            cancellationToken);
    }

    private ValueTask<bool> HandleBadRequestAsync(
        HttpContext httpContext,
        ArgumentException exception,
        CancellationToken cancellationToken)
    {
        return HandleAsync(
            httpContext,
            exception,
            StatusCodes.Status400BadRequest,
            "Bad Request",
            LogLevel.Warning,
            cancellationToken);
    }

    private ValueTask<bool> HandleConflictAsync(
        HttpContext httpContext,
        InvalidOperationException exception,
        CancellationToken cancellationToken)
    {
        return HandleAsync(
            httpContext,
            exception,
            StatusCodes.Status409Conflict,
            "Conflict",
            LogLevel.Warning,
            cancellationToken);
    }

    private ValueTask<bool> HandleUnexpectedAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        return HandleAsync(
            httpContext,
            exception,
            StatusCodes.Status500InternalServerError,
            "Internal Server Error",
            LogLevel.Error,
            cancellationToken,
            "An unexpected error occurred.");
    }

    private async ValueTask<bool> HandleAsync(
        HttpContext httpContext,
        Exception exception,
        int statusCode,
        string title,
        LogLevel logLevel,
        CancellationToken cancellationToken,
        string? detail = null)
    {
        logger.Log(
            logLevel,
            exception,
            "Request failed with status code {StatusCode}.",
            statusCode);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail ?? exception.Message,
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = httpContext.TraceIdentifier }
        }, cancellationToken);

        return true;
    }
}
