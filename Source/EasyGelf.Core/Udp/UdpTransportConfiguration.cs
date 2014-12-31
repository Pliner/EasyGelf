using System.Net;

namespace EasyGelf.Core.Udp
{
    public sealed class UdpTransportConfiguration : IUdpTransportConfiguration
    {
        public IPEndPoint Host { get; set; }
        public int MaxMessageSize { get; set; }
    }
}