using System.Net;

namespace EasyGelf.Core.Transports.Tcp
{
    public sealed class TcpTransportConfiguration
    {
        public IPEndPoint Host { get; set; }
    }
}