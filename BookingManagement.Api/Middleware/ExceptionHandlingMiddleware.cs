using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Api.Middleware
{
    public class ExceptionHandlingMiddleware
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    context.Response.ContentType = "application/json";

    switch (exception)
    {
        case ArgumentException:
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            break;

        case InvalidOperationException:
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            break;

        case DbUpdateConcurrencyException:
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            break;

        case KeyNotFoundException:
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            break;

        default:
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            break;
    }

    var response = new
    {
        message = exception.Message
    };

    await context.Response.WriteAsync(
        JsonSerializer.Serialize(response));
}
    }
}