using System.IO;
using System.Net.Security;
using System.Net.Sockets;

namespace EasyGelf.Core.Transports.Tcp
{
    public class TcpSslConnection : ITcpConnection
    {
        private readonly TcpTransportConfiguration configuration;
        
        private readonly TcpClient client;
        private SslStream sslStream;

        public TcpSslConnection(TcpTransportConfiguration configuration)
        {
            this.configuration = configuration;

            client = new TcpClient();
        }

        public void Open()
        {
            client.Connect(configuration.GetHost());
            sslStream = new SslStream(client.GetStream())
            {
                ReadTimeout = configuration.Timeout,
                WriteTimeout = configuration.Timeout
            };
            sslStream.AuthenticateAsClient(configuration.GetServerNameInCertificate());
        }

        public void Dispose()
        {
            if (sslStream != null)
            {
                sslStream.Close();
            }

            client.Close();
        }

        public Stream Stream
        {
            get { return sslStream; }
        }
    }
}