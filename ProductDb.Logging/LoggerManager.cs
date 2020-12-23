using NLog;

namespace ProductDb.Logging
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        public void LogDebug(string message)
        {
            logger.Debug(MessageFormatter(message));
        }

        public void LogError(string message)
        {
            logger.Error(MessageFormatter(message));
        }

        public void LogInfo(string message)
        {
            logger.Info(MessageFormatter(message));
        }

        public string MessageFormatter(string message)
        {
            return $"\nMessage => " + message;
        }
    }
}
