using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Transports;
using EasyGelf.Core.Transports.Udp;
using NLog.Targets;

namespace EasyGelf.NLog
{
    [Target("GelfUdp")]
    public sealed class GelfUdpTarget : GelfTargetBase
    {
        public GelfUdpTarget()
        {
            RemoteAddress = IPAddress.Loopback.ToString();
            RemotePort = 12201;
            MessageSize = 8096;
        }

        public string RemoteAddress { get; set; }

        public int RemotePort { get; set; }

        public int MessageSize { get; set; }

        protected override ITransport InitializeTransport(IEasyGelfLogger logger)
        {
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(new MessageBasedIdGenerator(), MessageSize.UdpMessageSize()));
            var configuration = new UdpTransportConfiguration
                {
                    RemoteAddress = RemoteAddress,
                    RemotePort = RemotePort
                };
            return new UdpTransport(configuration, encoder, new GelfMessageSerializer());
        }
    }
}