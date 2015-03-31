using System;

namespace EasyGelf.Core
{
    public interface IEasyGelfLogger
    {
        void Error(string message, Exception exception);
        void Debug(string message);
    }

    public sealed class SilentLogger : IEasyGelfLogger
    {
        public void Error(string message, Exception exception)
        {
        }

        public void Debug(string message)
        {
        }
    }
}