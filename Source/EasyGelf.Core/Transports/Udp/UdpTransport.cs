using System;
using System.Net.Sockets;
using EasyGelf.Core.Encoders;

namespace EasyGelf.Core.Transports.Udp
{
    public sealed class UdpTransport : ITransport
    {
        private readonly UdpTransportConfiguration configuration;
        private readonly ITransportEncoder encoder;
        private readonly IGelfMessageSerializer messageSerializer;
        private UdpClient udpClient;

        public UdpTransport(
            UdpTransportConfiguration configuration,
            ITransportEncoder encoder,
            IGelfMessageSerializer messageSerializer)
        {
            this.configuration = configuration;
            this.encoder = encoder;
            this.messageSerializer = messageSerializer;
        }

        private void EstablishConnection()
        {
            if (udpClient != null) return;
            var host = configuration.GetHost();
            try
            {
                udpClient = new UdpClient();
                udpClient.Connect(host);
            }
            catch (Exception exception)
            {
                Close();
                throw new CannotConnectException(string.Format("Cannot connect to {0}", host), exception);
            }
        }

        public void Send(GelfMessage message)
        {
            EstablishConnection();
            var serialzed = messageSerializer.Serialize(message);
            var encoded = encoder.Encode(serialzed);
            foreach (var bytes in encoded)
            {
                udpClient.Send(bytes, bytes.Length);
            }
        }

        public void Close()
        {
            if (udpClient == null) return;
            udpClient.Close();
            udpClient = null;
        }
    }
}