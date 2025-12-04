using log4net;
using log4net.Config;
using System.Reflection;

namespace LMSAPI.Helpers
{
    public class LoggerManager:ILoggerManager
    {
        private readonly ILog _logger;

        public LoggerManager()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());

            string configPath = Path.Combine(AppContext.BaseDirectory, "log4net.config");

            XmlConfigurator.Configure(logRepository, new FileInfo(configPath));

            _logger = LogManager.GetLogger(typeof(LoggerManager));

            _logger.Info("LoggerManager initialized successfully");
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
