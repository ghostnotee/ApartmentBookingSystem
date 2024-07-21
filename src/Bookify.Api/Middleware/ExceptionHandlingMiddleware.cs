using Bookify.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occurred: {Message}", e.Message);
            var exceptionDetails = GetExceptionDetails(e);
            var problemDetails = new ProblemDetails
            {
                Status = exceptionDetails.Status,
                Type = exceptionDetails.Type,
                Title = exceptionDetails.Title,
                Detail = exceptionDetails.Detail
            };
            if (exceptionDetails.Errors is not null) problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            context.Response.StatusCode = exceptionDetails.Status;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "ValidationFailure",
                "Validation error",
                "One or more validation errors occurred",
                validationException.Errors),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "ServerError",
                "ServerError",
                "An unexpected error has occurred",
                null
            )
        };
    }

    internal record ExceptionDetails(int Status, string Type, string Title, string Detail, IEnumerable<object>? Errors);
}