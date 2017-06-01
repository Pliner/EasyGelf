using EasyGelf.Core;
using EasyGelf.Core.Transports;
using EasyGelf.Core.Transports.Tcp;
using NLog.Targets;

namespace EasyGelf.NLog
{
    [Target("GelfTcp")]
    public sealed class GelfTcpTarget : GelfTargetBase
    {
        public GelfTcpTarget()
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