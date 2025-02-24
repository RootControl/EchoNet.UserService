using System.Security.Claims;
using Application.Commands;
using Application.DTOs;
using MediatR;
using MongoDB.Driver;

namespace Presentation.Endpoints;

public static class ProfileEndpoint
{
    public static void MapProfileEndpoints(this WebApplication app)
    {
        app.MapGet("/profile", async (HttpContext context, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guidId))
                return Results.Unauthorized();

            var result = await mediator.Send(new GetProfileCommand(guidId), cancellationToken);
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapPut("/profile", async (HttpContext context, UpdateProfileCommand command, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guidId))
                    return Results.Unauthorized();

                var updatedCommand = command with { UserId = guidId };
                var result = await mediator.Send(updatedCommand, cancellationToken);
                return Results.Ok(result);
            }).RequireAuthorization();
    }
}