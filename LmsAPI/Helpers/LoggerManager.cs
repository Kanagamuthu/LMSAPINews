using log4net;
using log4net.Config;
using System.Reflection;

namespace LMSAPI.Helpers
{
    public class LoggerManager:ILoggerManager
    {
        private readonly ILog _logger;

        public LoggerManager()
        {// Configure log4net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            _logger = LogManager.GetLogger(typeof(LoggerManager));

            // Log startup message
            _logger.Info("LoggerManager initialized successfully");
            _logger = LogManager.GetLogger(typeof(LoggerManager));
        }

        public void LogInfo(string message)
        {
            _logger.Info(message);
        }
        public void LogWarn(string message)
        {
            _logger.Warn(message);
        }
        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }
        public void LogError(string message)
        {
            _logger.Error(message);
        }
        public void LogError(Exception ex, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger.Error(ex.Message, ex);
            }
            else
            {
                _logger.Error($"{message} - {ex.Message}", ex);
            }
        }

        public void LogFatal(string message)
        {
            _logger.Fatal(message);
        }
    }
}
