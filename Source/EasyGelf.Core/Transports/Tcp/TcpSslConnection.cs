using System.IO;
using System.Net.Security;
using System.Net.Sockets;

namespace EasyGelf.Core.Transports.Tcp
{
    public class TcpSslConnection : ITcpConnection
    {
        private readonly TcpSslTransportConfiguration configuration;
        
        private readonly TcpClient client;
        private SslStream sslStream;

        public TcpSslConnection(TcpSslTransportConfiguration configuration)
        {
            this.configuration = configuration;

            client = new TcpClient();
        }

        public void Open()
        {
            client.Connect(configuration.Host);
            sslStream = new SslStream(client.GetStream())
            {
                ReadTimeout = configuration.Timeout,
                WriteTimeout = configuration.Timeout
            };
            sslStream.AuthenticateAsClient(configuration.ServerNameInCertificate);
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