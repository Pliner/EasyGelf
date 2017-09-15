using System;

namespace EasyGelf.Core
{
    public interface IEasyGelfLogger
    {
        void Error(string message, Exception exception);
        void Debug(string message);
        void SetException(Exception e);
        void CheckException();
    }

    public sealed class SilentLogger : IEasyGelfLogger
    {
        private static Exception threadException = null;

        public void Error(string message, Exception exception)
        {
        }

        public void Debug(string message)
        {
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