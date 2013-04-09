using System;

namespace Regalo.Core
{
    public interface ILogger
    {
        void Debug(object sender, string format, params object[] args);
        void Info(object sender, string format, params object[] args);
        void Warn(object sender, string format, params object[] args);
        void Error(object sender, Exception exception, string format, params object[] args);
    }
}