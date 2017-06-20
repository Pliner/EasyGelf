using System.Linq;
using System.Net;

namespace EasyGelf.Core.Transports
{
    using System.Threading.Tasks;

    public class IpTransportConfiguration
    {
        private IPEndPoint host;


        public string RemoteAddress { get; set; }

        public int RemotePort { get; set; }


        public async Task<IPEndPoint> GetHost()
        {
            if (host != null)
            {
                return host;
            }

            var remoteIpAddress = (await Dns.GetHostAddressesAsync(RemoteAddress))
                .Shuffle()
                .DefaultIfEmpty(IPAddress.Loopback)
                .First();
            
            host = new IPEndPoint(remoteIpAddress, RemotePort);
            return host;
        }
    }
}