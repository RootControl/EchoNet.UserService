using Serilog;

namespace Infrastructure.Logging;

public static class SerilogSetup
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.File("logs/userservice-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}