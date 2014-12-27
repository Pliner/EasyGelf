using System.Net;
using System.Net.Sockets;

namespace EasyGelf.Core
{
    public class UdpTransport : TransportBase
    {
        private readonly UdpClient udpClient;

        public UdpTransport(ITransportConfiguration configuration) : base(configuration)
        {
            udpClient = new UdpClient();
        }

        protected override bool SplitLargeMessage
        {
            get { return true; }
        }

        public override void SendToEndpoint(IPEndPoint endPoint, byte[] bytes)
        {
            udpClient.Send(bytes, bytes.Length, endPoint);
        }

        public override void Close()
        {
            udpClient.Close();
        }
    }
}