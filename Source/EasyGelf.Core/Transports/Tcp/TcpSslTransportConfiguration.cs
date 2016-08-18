using System.Net;

namespace EasyGelf.Core.Transports.Tcp
{
    public class TcpSslTransportConfiguration : TcpTransportConfiguration
    {
        public string ServerNameInCertificate { get; set; }

        public int Timeout { get; set; }
    }
}