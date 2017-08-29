using System;
using System.Net.Sockets;
using EasyGelf.Core.Encoders;

namespace EasyGelf.Core.Transports.Udp
{
    public sealed class UdpTransport : ITransport, IDisposable
    {
        private readonly UdpTransportConfiguration configuration;
        private readonly ITransportEncoder encoder;
        private readonly IGelfMessageSerializer messageSerializer;
        private readonly UdpClient udpClient;
        private bool disposed;

        public UdpTransport(
            UdpTransportConfiguration configuration,
            ITransportEncoder encoder,
            IGelfMessageSerializer messageSerializer)
        {
            this.configuration = configuration;
            this.encoder = encoder;
            this.messageSerializer = messageSerializer;
            this.udpClient = new UdpClient();
        }

        public void Send(GelfMessage message)
        {
            var serialzed = messageSerializer.Serialize(message);
            var encoded = encoder.Encode(serialzed);
            foreach (var bytes in encoded)
            {
                udpClient.Send(bytes, bytes.Length, configuration.GetHost());
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                udpClient.Close();
                disposed = true;
            }
        }
    }
}