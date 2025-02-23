namespace Presentation.Configurations;

public static class SecurityHeadersConfiguration
{
    public static void ConfigureSecurityHeaders(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000");
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
            await next();
        });
    }
}