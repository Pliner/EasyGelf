using System.Net;

namespace EasyGelf.Core.Udp
{
    public class UdpTransportConfiguration : AbstractTransportConfiguration, IUdpTransportConfiguration
    {
        public IPEndPoint Host { get; set; }
    }
}