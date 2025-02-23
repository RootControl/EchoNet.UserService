using MediatR;
using Application.Commands;

namespace Presentation.Endpoints;

public static class RegisterEndpoint
{
    public static void RegisterUserEndpoint(this WebApplication app)
    {
        app.MapPost("/register", async (RegisterCommand command, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });
    }
}