using System.Security.Claims;
using Application.Commands;
using MediatR;

namespace Presentation.Endpoints;

public static class LogoutEndpoint
{
    public static void MapLogoutEndpoint(this WebApplication app)
    {
        app.MapPost("/logout", async (IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guidId))
                return Results.Unauthorized();
            
            await mediator.Send(new LogoutCommand(guidId), cancellationToken);
            return Results.Ok();
        }).RequireAuthorization();
    }
}