using System;
using EasyGelf.Core;
using NLog.Common;

namespace EasyGelf.NLog
{
    public sealed class VerboseLogger : IEasyGelfLogger
    {
        public void Error(string message, Exception exception)
        {
            InternalLogger.Error(string.Format("{0} ---> {1}", message, exception));
        }

        public void Debug(string message)
        {
            InternalLogger.Debug(message);
        }
    }
}