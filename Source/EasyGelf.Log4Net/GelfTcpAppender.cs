using System.Linq;
using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Tcp;
using JetBrains.Annotations;

namespace EasyGelf.Log4Net
{
    public sealed class GelfTcpAppender : GelfAppenderBase
    {
        public GelfTcpAppender()
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
            var remoteIpAddress = Dns.GetHostAddresses(RemoteAddress)
                .Shuffle()
                .FirstOrDefault() ?? IPAddress.Loopback;
            var configuration = new TcpTransportConfiguration
                {
                    Host = new IPEndPoint(remoteIpAddress, RemotePort),
                };
            return new TcpTransport(configuration, new GelfMessageSerializer());
        }
    }
}