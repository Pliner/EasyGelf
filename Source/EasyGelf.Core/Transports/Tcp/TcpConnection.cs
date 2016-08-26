using System.IO;
using System.Net.Sockets;

namespace EasyGelf.Core.Transports.Tcp
{
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

        public void Open()
        {
            client.Connect(configuration.GetHost());
            networkStream = client.GetStream();
        }

        public void Dispose()
        {
            if (networkStream != null)
            {
                networkStream.Close();
            }

            client.Close();
        }

        public Stream Stream
        {
            get { return networkStream; }
        }
    }
}
