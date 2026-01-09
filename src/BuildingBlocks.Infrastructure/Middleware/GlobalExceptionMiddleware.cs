using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            var problem = CreateProblem(
                context,
                StatusCodes.Status400BadRequest,
                title: "One or more validation errors occurred.",
                detail: "See the errors property for details.");

            problem.Extensions["errors"] = errors;

            await WriteProblem(context, problem);
        }
        catch (InvalidOperationException ex)
        {
            // Business rule violation (mapped centrally)
            _logger.LogWarning(
                ex,
                "Business rule violation. CorrelationId={CorrelationId}",
                context.Items["CorrelationId"]);

            var problem = CreateProblem(
                context,
                StatusCodes.Status409Conflict,
                title: "Business rule violation",
                detail: ex.Message);

            await WriteProblem(context, problem);
        }
        catch (KeyNotFoundException ex)
        {
            var problem = CreateProblem(
                context,
                StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: ex.Message);

            await WriteProblem(context, problem);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception. CorrelationId={CorrelationId}",
                context.Items["CorrelationId"]);

            var problem = CreateProblem(
                context,
                StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "An unexpected error occurred.");

            await WriteProblem(context, problem);
        }
    }

    private static ProblemDetails CreateProblem(
        HttpContext context,
        int status,
        string title,
        string detail)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{status}",
            Instance = context.Request.Path
        };

        problem.Extensions["correlationId"] = context.Items["CorrelationId"];
        return problem;
    }

    private static async Task WriteProblem(
        HttpContext context,
        ProblemDetails problem)
    {
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}
