using System.Net;

namespace EasyGelf.Core.Udp
{
    public sealed class UdpTransportConfiguration : AbstractTransportConfiguration, IUdpTransportConfiguration
    {
        public IPEndPoint Host { get; set; }
    }
}