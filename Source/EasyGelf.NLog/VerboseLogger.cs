using System;
using EasyGelf.Core;
using NLog.Common;

namespace EasyGelf.NLog
{
    public sealed class VerboseLogger : IEasyGelfLogger
    {
        private static Exception threadException = null;
        public void Error(string message, Exception exception)
        {
            InternalLogger.Error(string.Format("{0} ---> {1}", message, exception));
        }

        public void Debug(string message)
        {
            InternalLogger.Debug(message);
        }
        public void SetException(Exception exception)
        {
            threadException = exception;
        }
        public void CheckException()
        {
            Exception e = threadException;
            if (threadException != null)
            {
                threadException = null;
                throw e;
            }
        }
    }
}