using System.Net;
using JetBrains.Annotations;

namespace EasyGelf.Core.Udp
{
    public interface IUdpTransportConfiguration : IAbstractTransportConfiguration
    {
        [NotNull]
        IPEndPoint Host { get; }
    }
}