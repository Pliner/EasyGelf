using System.Net;
using JetBrains.Annotations;

namespace EasyGelf.Core.Tcp
{
    public sealed class TcpTransportConfiguration
    {
        [NotNull]
        public IPEndPoint Host { get; set; }
    }
}