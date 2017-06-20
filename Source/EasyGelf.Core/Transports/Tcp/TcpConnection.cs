using System.IO;
using System.Net.Sockets;

namespace EasyGelf.Core.Transports.Tcp
{
    using System.Threading.Tasks;

    public class TcpConnection : ITcpConnection
    {
        private readonly TcpTransportConfiguration configuration;
        
        private readonly TcpClient client;
        private NetworkStream networkStream;

        public TcpConnection(TcpTransportConfiguration configuration)
        {
            this.configuration = configuration;

            client = new TcpClient();
        }

        public async Task Open()
        {
            var host = await this.configuration.GetHost();
            await client.ConnectAsync(host.Address, host.Port);
            networkStream = client.GetStream();
        }

        public void Dispose()
        {
            this.networkStream?.Dispose();
            this.client.SafeDispose();
        }

        public Stream Stream => networkStream;
    }
}
