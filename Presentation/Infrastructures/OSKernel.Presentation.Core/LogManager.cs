using log4net;
using System;

namespace XYKernel.Presentation.Core
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public class LogManager
    {
        private ILog _log { get; set; }

        private static readonly object _lock = new object();

        private static LogManager _logManager;

        public static LogManager Logger
        {
            get
            {
                if (_logManager == null)
                {
                    lock (_lock)
                    {
                        _logManager = new LogManager();
                    }
                }

                return _logManager;
            }
        }

        public LogManager()
        {
            string log4netConfig = AppDomain.CurrentDomain.BaseDirectory + "log4net.config";
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(log4netConfig);
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            _log = log4net.LogManager.GetLogger("ClientLog");
        }

        #region Method

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        #endregion
    }
}
