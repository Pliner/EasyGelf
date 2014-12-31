using System.Net.Sockets;

namespace EasyGelf.Core.Udp
{
    public sealed class UdpTransport : AbstractTransport
    {
        private readonly IUdpTransportConfiguration configuration;
        private readonly UdpClient udpClient;

        public UdpTransport(IUdpTransportConfiguration configuration)
            : base(configuration)
        {
            this.configuration = configuration;
            udpClient = new UdpClient();
        }

        protected override void SendInternal(byte[] bytes)
        {
            udpClient.Send(bytes, bytes.Length, configuration.Host);
        }

        public override void Close()
        {
            udpClient.Close();
        }
    }
}