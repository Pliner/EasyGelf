using System.Linq;
using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Transports;
using EasyGelf.Core.Transports.Tcp;

namespace EasyGelf.Log4Net
{
    public sealed class GelfTcpAppender : GelfAppenderBase
    {
        public GelfTcpAppender()
        {
            RemoteAddress = IPAddress.Loopback.ToString();
            RemotePort = 12201;
        }

        public string RemoteAddress { get; set; }

        public int RemotePort { get; set; }

        public bool Ssl { get; set; }

        public int Timeout { get; set; }

        protected override ITransport InitializeTransport(IEasyGelfLogger logger)
        {
            var remoteIpAddress = Dns.GetHostAddresses(RemoteAddress)
                .Shuffle()
                .FirstOrDefault() ?? IPAddress.Loopback;
            var ipEndPoint = new IPEndPoint(remoteIpAddress, RemotePort);

            if (Ssl)
            {
                var configuration = new TcpSslTransportConfiguration
                {
                    Host = ipEndPoint,
                    ServerNameInCertificate = RemoteAddress,
                    Timeout = Timeout
                };
                return new TcpTransport(configuration, new GelfMessageSerializer(), () => new TcpSslConnection(configuration));
            }
            else
            {
                var configuration = new TcpTransportConfiguration
                {
                    Host = ipEndPoint
                };
                return new TcpTransport(configuration, new GelfMessageSerializer(), () => new TcpConnection(configuration));
            }
        }
    }
}