using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Udp;
using JetBrains.Annotations;
using NLog.Targets;

namespace EasyGelf.NLog
{
    [Target("GelfUdp")]
    public sealed class GelfUdpTarget : GelfTargetBase
    {
        private const int UdpMessageSize = 1024;

        public GelfUdpTarget()
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
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(UdpMessageSize));
            var removeIpAddress = IPAddress.Parse(RemoteAddress);
            var configuration = new UdpTransportConfiguration
                {
                    Host = new IPEndPoint(removeIpAddress, RemotePort),
                };
            return new UdpTransport(configuration, encoder);
        }
    }
}