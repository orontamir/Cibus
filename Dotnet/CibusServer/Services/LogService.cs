using Serilog;

namespace CibusServer.Services
{
    public class LogService
    {
      
        public static void LogInformation(string message)
        {
            Log.Information(message);
        }

        public static void LogWarning(string message) { Log.Warning(message); }
        public static void LogError(string message) { Log.Error(message); }
        public static void LogFatal(string message) { LogFatal(message); }
    }
}
