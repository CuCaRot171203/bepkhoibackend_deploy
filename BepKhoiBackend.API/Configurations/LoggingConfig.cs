using Serilog;

namespace BepKhoiBackend.API.Configurations
{
    public static class LoggingConfig
    {
        public static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
