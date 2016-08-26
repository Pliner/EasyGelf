using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using System.Linq;
using EasyGelf.Core.Transports;
using EasyGelf.Core.Transports.Udp;

namespace EasyGelf.Log4Net
{
    public sealed class GelfUdpAppender : GelfAppenderBase
    {
        public GelfUdpAppender()
        {
            RemoteAddress = IPAddress.Loopback.ToString();
            RemotePort = 12201;
            MessageSize = 8096;
        }

        public int MessageSize { get; set; }

        public string RemoteAddress { get; set; }

        public int RemotePort { get; set; }

        protected override ITransport InitializeTransport(IEasyGelfLogger logger)
        {
            var remoteIpAddress = Dns.GetHostAddresses(RemoteAddress)
                .Shuffle()
                .DefaultIfEmpty(IPAddress.Loopback)
                .First();
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(new MessageBasedIdGenerator(), MessageSize.UdpMessageSize()));
            var configuration = new UdpTransportConfiguration
                {
                    Host = new IPEndPoint(remoteIpAddress, RemotePort),
                };
            return new UdpTransport(configuration, encoder, new GelfMessageSerializer());
        }
    }
}