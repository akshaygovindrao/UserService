using Microsoft.AspNetCore.Mvc;
using UserService.Common.Exceptions;

namespace UserService.Middleware;

public class GlobalExceptionHandler
{
    #region Fields

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    #endregion

    #region Constructors

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    #endregion

    #region Public Methods

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            var (statusCode, title) = exception switch
            {
                UserAlreadyExistsException => (StatusCodes.Status409Conflict, exception.Message),
                InvalidCredentialsException => (StatusCodes.Status401Unauthorized, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception processing {Method} {Path}",
                    httpContext.Request.Method,
                    httpContext.Request.Path);
            }

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = statusCode,
                Title = title,
            });
        }
    }

    #endregion
}
