using System;
using Regalo.Core;

namespace Regalo.Testing
{
    public class ConsoleLogger : ILogger
    {
        public void Debug(object sender, string format, params object[] args)
        {
            Log(sender, format, args);
        }

        public void Info(object sender, string format, params object[] args)
        {
            Log(sender, format, args);
        }

        public void Warn(object sender, string format, params object[] args)
        {
            Log(sender, format, args);
        }

        public void Error(object sender, Exception exception, string format, params object[] args)
        {
            Log(sender, format, args);
            Console.WriteLine(exception);
        }

        private void Log(object sender, string format, params object[] args)
        {
            var time = DateTimeOffset.Now;
            var message = string.Format(format, args);
            var log = string.Format("{0:s}: {1}: {2}", time, sender, message);

            Console.WriteLine(log);
        }
    }
}