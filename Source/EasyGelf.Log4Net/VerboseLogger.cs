using System;
using EasyGelf.Core;
using log4net.Util;

namespace EasyGelf.Log4Net
{
    public sealed class VerboseLogger : IEasyGelfLogger
    {
        public void Error(string message, Exception exception)
        {
            LogLog.Error(message, exception);
        }

        public void Debug(string message)
        {
            LogLog.Debug(message);
        }
    }
}