using System.Net.Sockets;
using EasyGelf.Core.Encoders;

namespace EasyGelf.Core.Udp
{
    public sealed class UdpTransport : ITransport
    {
        private readonly UdpTransportConfiguration configuration;
        private readonly ITransportEncoder encoder;
        private readonly IGelfMessageSerializer messageSerializer;
        private UdpClient udpClient;

        public UdpTransport(UdpTransportConfiguration configuration, ITransportEncoder encoder, IGelfMessageSerializer messageSerializer)
        {
            this.configuration = configuration;
            this.encoder = encoder;
            this.messageSerializer = messageSerializer;
            udpClient = new UdpClient();
        }

        public void Send(GelfMessage message)
        {
            foreach (var bytes in encoder.Encode(messageSerializer.Serialize(message)))
            {
                udpClient.Send(bytes, bytes.Length, configuration.Host);
            }
        }

        public void Close()
        {
            if (udpClient == null)
                return;
            udpClient.SafeDispose();
            udpClient = null;
        }
    }
}