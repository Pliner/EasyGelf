using System;

namespace EasyGelf.Core
{
    public interface IEasyGelfLogger
    {
        void Error(string message, Exception exception);
        void Debug(string message);
    }
}