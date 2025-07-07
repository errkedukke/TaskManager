using System.Net;
using TaskManager.API.Models;
using TaskManager.Application.Exceptions;

namespace TaskManager.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var problem = new CustomProblemDetails();

        switch (ex)
        {
            case BadRequestException badRequest:
                statusCode = HttpStatusCode.BadRequest;
                problem.Title = badRequest.Message;
                problem.Status = (int)statusCode;
                problem.Detail = badRequest.InnerException?.Message;
                problem.Type = nameof(BadRequestException);
                problem.Errors = badRequest.ValidationErrors;
                break;
            case NotFoundException notFound:
                statusCode = HttpStatusCode.NotFound;
                problem.Title = notFound.Message;
                problem.Status = (int)statusCode;
                problem.Detail = notFound.InnerException?.Message;
                problem.Type = nameof(NotFoundException);
                break;
            default:
                statusCode = HttpStatusCode.BadRequest;
                problem.Title = ex.Message;
                problem.Status = (int)statusCode;
                problem.Detail = ex.StackTrace;
                problem.Type = nameof(BadRequestException);
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(problem);
    }
}
