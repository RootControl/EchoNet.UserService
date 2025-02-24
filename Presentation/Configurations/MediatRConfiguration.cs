using Application.Behaviors;
using Application.Handlers;
using MediatR;

namespace Presentation.Configurations;

public static class MediatRConfiguration
{
    public static void RegisterMediatR(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly);
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
    }
}