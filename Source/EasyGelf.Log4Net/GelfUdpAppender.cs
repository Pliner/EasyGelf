using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Udp;
using JetBrains.Annotations;

namespace EasyGelf.Log4Net
{
    public sealed class GelfUdpAppender : GelfAppenderBase
    {
        [UsedImplicitly]
        public IPAddress RemoteAddress { get; set; }
        [UsedImplicitly]
        public int RemotePort { get; set; }

        protected override ITransport InitializeTransport()
        {
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(1024));
            var configuration = new UdpTransportConfiguration
                {
                    Host = new IPEndPoint(RemoteAddress, RemotePort),
                };
            return new UdpTransport(configuration, encoder);
        }
    }
}