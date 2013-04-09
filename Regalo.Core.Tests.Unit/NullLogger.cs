using System;

namespace Regalo.Core.Tests.Unit
{
    public class NullLogger : ILogger
    {
        public void Debug(object sender, string format, params object[] args)
        { }

        public void Info(object sender, string format, params object[] args)
        { }

        public void Warn(object sender, string format, params object[] args)
        { }

        public void Error(object sender, Exception exception, string format, params object[] args)
        { }
    }
}