using System.Linq;
using System.Net;

namespace EasyGelf.Core.Transports
{
    public class IpTransportConfiguration
    {
        private IPEndPoint host;


        public string RemoteAddress { get; set; }

        public int RemotePort { get; set; }


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
    }
}