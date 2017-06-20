using System.IO;
using System.Net.Security;
using System.Net.Sockets;

namespace EasyGelf.Core.Transports.Tcp
{
    using System.Threading.Tasks;

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

        public async Task Open()
        {
            var host = await this.configuration.GetHost();
            await client.ConnectAsync(host.Address, host.Port);
            sslStream = new SslStream(client.GetStream())
            {
                ReadTimeout = configuration.Timeout,
                WriteTimeout = configuration.Timeout
            };
            await sslStream.AuthenticateAsClientAsync(configuration.GetServerNameInCertificate());
        }

        public void Dispose()
        {
            this.sslStream?.Dispose();

            this.client.SafeDispose();
        }

        public Stream Stream => sslStream;
    }
}