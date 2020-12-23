using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Service.LoggingService
{
    public interface ILoggingService
    {
        void LogDebug(string message);
        void LogError(string message);
        void LogInfo(string message);
        string MessageFormatter(string message);
    }
}
