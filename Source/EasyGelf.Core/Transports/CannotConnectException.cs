using System;

namespace EasyGelf.Core.Transports
{
    public sealed class CannotConnectException : Exception
    {
        public CannotConnectException(string message, Exception exception) : base(message, exception)
        {
        }

        public CannotConnectException(string message) : base(message)
        {
        }
    }
}