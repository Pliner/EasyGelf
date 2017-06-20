using System;
using System.IO;

namespace EasyGelf.Core.Transports.Tcp
{
    using System.Threading.Tasks;

    public interface ITcpConnection : IDisposable
    {
        Task Open();

        Stream Stream { get; }
    }
}
