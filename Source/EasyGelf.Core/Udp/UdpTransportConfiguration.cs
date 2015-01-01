using System.Net;
using JetBrains.Annotations;

namespace EasyGelf.Core.Udp
{
    public sealed class UdpTransportConfiguration
    {
        [NotNull]
        public IPEndPoint Host { get; set; }
    }
}