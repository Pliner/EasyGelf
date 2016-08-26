using System.Linq;
using System.Net;

namespace EasyGelf.Core.Transports.Tcp
{
    public sealed class TcpTransportConfiguration
    {
        private IPEndPoint host;
        
        public string RemoteAddress { get; set; }

        public int RemotePort { get; set; }

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

        public IPEndPoint GetHost()
        {
            if (host != null)
            {
                return host;
            }

            var remoteIpAddress = Dns.GetHostAddresses(RemoteAddress)
                .Shuffle()
                .DefaultIfEmpty(IPAddress.Loopback)
                .First();
            
            host = new IPEndPoint(remoteIpAddress, RemotePort);
            return host;
        }

        public string GetServerNameInCertificate()
        {
            return RemoteAddress;
        }
    }
}