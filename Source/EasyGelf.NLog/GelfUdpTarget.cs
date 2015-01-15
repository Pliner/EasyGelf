using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Udp;
using JetBrains.Annotations;
using NLog.Targets;
using System.Linq;

namespace EasyGelf.NLog
{
    [Target("GelfUdp")]
    public sealed class GelfUdpTarget : GelfTargetBase
    {
        public GelfUdpTarget()
        {
            RemoteAddress = IPAddress.Loopback.ToString();
            RemotePort = 12201;
            MessageSize = 1024;
        }

        [UsedImplicitly]
        public string RemoteAddress { get; set; }

        [UsedImplicitly]
        public int RemotePort { get; set; }

        [UsedImplicitly]
        public int MessageSize { get; set; }

        protected override ITransport InitializeTransport()
        {
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(new MessageBasedIdGenerator(), MessageSize.UdpMessageSize()));
            var removeIpAddress = Dns.GetHostAddresses(RemoteAddress)
                .Shuffle()
                .FirstOrDefault() ?? IPAddress.Loopback;
            var configuration = new UdpTransportConfiguration
                {
                    Host = new IPEndPoint(removeIpAddress, RemotePort),
                };
            return new UdpTransport(configuration, encoder, new GelfMessageSerializer());
        }
    }
}