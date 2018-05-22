using System;
using NLog;

namespace BsuBot.Logger
{
    [Serializable]
    public class LoggerService<T> : ILoggerService<T>
    {
        [NonSerialized]
        public ILogger logger;

        public LoggerService()
        {
            logger = LogManager.GetLogger(typeof(T).FullName);
        }

        public void Debug(string message)
        {
            logger.Debug(message);
            
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Error(Exception message)
        {
            logger.Error(message.ToString());
        }

        public void Warn(string message)
        {
            logger.Warn(message);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }

      

    }
}
