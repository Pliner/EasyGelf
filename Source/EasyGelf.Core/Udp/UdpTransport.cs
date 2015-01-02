using System.Net.Sockets;
using System.Text;
using EasyGelf.Core.Encoders;

namespace EasyGelf.Core.Udp
{
    public sealed class UdpTransport : ITransport
    {
        private readonly UdpTransportConfiguration configuration;
        private readonly ITransportEncoder encoder;

        public UdpTransport(UdpTransportConfiguration configuration, ITransportEncoder encoder)
        {
            this.configuration = configuration;
            this.encoder = encoder;
        }

        public void Send(GelfMessage message)
        {
            using (var udpClient = new UdpClient())
            {
                foreach (var bytes in encoder.Encode(Encoding.UTF8.GetBytes(message.Serialize())))
                {
                    udpClient.Send(bytes, bytes.Length, configuration.Host);
                }
            }
        }

        public void Close()
        {
        }
    }
}