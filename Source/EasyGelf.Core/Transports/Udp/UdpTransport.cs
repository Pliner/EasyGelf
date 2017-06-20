using System.Net.Sockets;
using EasyGelf.Core.Encoders;

namespace EasyGelf.Core.Transports.Udp
{
    using System.Threading.Tasks;

    public sealed class UdpTransport : ITransport
    {
        private readonly UdpTransportConfiguration configuration;
        private readonly ITransportEncoder encoder;
        private readonly IGelfMessageSerializer messageSerializer;

        public UdpTransport(UdpTransportConfiguration configuration, ITransportEncoder encoder, IGelfMessageSerializer messageSerializer)
        {
            this.configuration = configuration;
            this.encoder = encoder;
            this.messageSerializer = messageSerializer;
        }

        public async Task Send(GelfMessage message)
        {
            using (var udpClient = new UdpClient())
                foreach (var bytes in await encoder.Encode(messageSerializer.Serialize(message)))
                    await udpClient.SendAsync(bytes, bytes.Length, await configuration.GetHost());
        }

        public void Close()
        {
        }
    }
}