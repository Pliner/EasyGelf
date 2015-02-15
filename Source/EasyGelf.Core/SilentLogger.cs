using System;

namespace EasyGelf.Core
{
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