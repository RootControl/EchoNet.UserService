using Application.Handlers;

namespace Presentation.Configurations;

public static class MediatRConfiguration
{
    public static void RegisterMediatR(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly));
    }
}