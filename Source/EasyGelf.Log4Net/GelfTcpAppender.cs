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
            TcpTransportConfiguration defaultCfg = TcpTransportConfiguration.GetDefaultConfiguration();
            
            RemoteAddress = defaultCfg.RemoteAddress;
            RemotePort = defaultCfg.RemotePort;
            Ssl = defaultCfg.Ssl;
            Timeout = defaultCfg.Timeout;
        }

        public string RemoteAddress { get; set; }

        public int RemotePort { get; set; }

        public bool Ssl { get; set; }

        public int Timeout { get; set; }

        protected override ITransport InitializeTransport(IEasyGelfLogger logger)
        {
            var configuration = new TcpTransportConfiguration
            {
                RemoteAddress = RemoteAddress,
                RemotePort = RemotePort,
                Ssl = Ssl,
                Timeout = Timeout
            };

            return TcpTransportFactory.Produce(configuration);
        }
    }
}