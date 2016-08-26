using System.Net;

namespace EasyGelf.Core.Transports.Tcp
{
    public sealed class TcpTransportConfiguration : IpTransportConfiguration
    {
        public bool Ssl { get; set; }
        
        public int Timeout { get; set; }

        public static TcpTransportConfiguration GetDefaultConfiguration()
        {
            return new TcpTransportConfiguration
            {
                RemoteAddress = IPAddress.Loopback.ToString(),
                RemotePort = 12201,
                Timeout = 30000,
            };
        }

        public string GetServerNameInCertificate()
        {
            return RemoteAddress;
        }
    }
}