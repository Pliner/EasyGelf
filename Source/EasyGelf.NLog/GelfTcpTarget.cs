using System.Linq;
using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Tcp;
using JetBrains.Annotations;
using NLog.Targets;

namespace EasyGelf.NLog
{
    [Target("GelfTcp")]
    public sealed class GelfTcpTarget : GelfTargetBase
    {
        public GelfTcpTarget()
        {
            RemoteAddress = IPAddress.Loopback.ToString();
            RemotePort = 12201;
        }

        [UsedImplicitly]
        public string RemoteAddress { get; set; }

        [UsedImplicitly]
        public int RemotePort { get; set; }

        protected override ITransport InitializeTransport()
        {
            var removeIpAddress = Dns.GetHostAddresses(RemoteAddress)
                                     .Shuffle()
                                     .FirstOrDefault() ?? IPAddress.Loopback;
            var configuration = new TcpTransportConfiguration
                {
                    Host = new IPEndPoint(removeIpAddress, RemotePort),
                };
            return new TcpTransport(configuration, new GelfMessageSerializer());
        }
    }
}