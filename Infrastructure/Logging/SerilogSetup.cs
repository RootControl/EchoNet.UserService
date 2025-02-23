using Serilog;

namespace Infrastructure.Logging;

public static class SerilogSetup
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }
}