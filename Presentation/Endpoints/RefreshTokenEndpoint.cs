using Application.Commands;
using MediatR;

namespace Presentation.Endpoints;

public static class RefreshTokenEndpoint
{
    public static void MapRefreshTokenEndpoint(this WebApplication app)
    {
        app.MapPost("/refresh-token", async (RefreshTokenCommand command, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });
    }
}