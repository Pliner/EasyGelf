using System.Net;
using JetBrains.Annotations;

namespace EasyGelf.Core.Udp
{
    public interface IUdpTransportConfiguration
    {
        [NotNull]
        IPEndPoint Host { get; }
    }
}