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
        public static ITransport Produce(TcpTransportConfiguration configuration)
        {
            if (configuration.Ssl)
            {
                return new TcpTransport(configuration, new GelfMessageSerializer(), () => new TcpSslConnection(configuration));
            }
            else
            {
                return new TcpTransport(configuration, new GelfMessageSerializer(), () => new TcpConnection(configuration));
            }
        }
    }
}
