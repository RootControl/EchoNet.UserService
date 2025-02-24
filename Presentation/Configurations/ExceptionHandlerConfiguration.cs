using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Presentation.Configurations;

public static class ExceptionHandlerConfiguration
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                switch (exception)
                {
                    case ValidationException validationException:
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(new { error = $"Validation failed: {string.Join(", ", validationException.Errors.Select(e => e.ErrorMessage))}" });
                        break;
                    case UnauthorizedAccessException:
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new { error = "Invalid credentials." });
                        break;
                    case KeyNotFoundException:
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsJsonAsync(exception.Message);
                        break;
                    case ArgumentException:
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(new { error = exception.Message });
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsJsonAsync(new { error = "An error occurred." });
                        break;
                }
            });
        });
    }
}