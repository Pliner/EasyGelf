using System;
using System.IO;

namespace EasyGelf.Core.Transports.Tcp
{
    public interface ITcpConnection : IDisposable
    {
        void Open();

        Stream Stream { get; }
    }
}
