using System.Net;
using EasyGelf.Core;
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
            return new UdpTransport(new UdpTransportConfiguration
            {
                MaxMessageSize = 1024,
                Host = new IPEndPoint(RemoteAddress, RemotePort)
            });
        }
    }
}