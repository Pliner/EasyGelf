using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace EasyGelf.Core.Transports.Tcp
{
    public class TcpTransportFactory
    {
        public static ITransport Produce(TcpTransportConfiguration configuration, IEasyGelfLogger logger)
        {
            return configuration.Ssl
                ? new TcpTransport(configuration, new GelfMessageSerializer(logger), () => new TcpSslConnection(configuration))
                : new TcpTransport(configuration, new GelfMessageSerializer(logger), () => new TcpConnection(configuration));
        }
    }
}
