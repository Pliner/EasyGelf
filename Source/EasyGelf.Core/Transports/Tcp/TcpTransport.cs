using System;

namespace EasyGelf.Core.Transports.Tcp
{
    public sealed class TcpTransport : ITransport
    {
        private readonly TcpTransportConfiguration configuration;
        private readonly IGelfMessageSerializer messageSerializer;
        private readonly Func<ITcpConnection> createConnection;
        private ITcpConnection connection;

        public TcpTransport(TcpTransportConfiguration configuration,
                            IGelfMessageSerializer messageSerializer,
                            Func<ITcpConnection> createConnection)
        {
            this.configuration = configuration;
            this.messageSerializer = messageSerializer;
            this.createConnection = createConnection;
        }


        private void EstablishConnection()
        {
            if (connection != null)
            {
                return;
            }

            try
            {
                connection = createConnection();
                connection.Open();
            }
            catch (Exception exception)
            {
                Close();

                throw new CannotConnectException(string.Format("Cannot connect to {0}", configuration.GetHost()), exception);
            }
        }


        public void Send(GelfMessage message)
        {
            EstablishConnection();
            
            var bytes = messageSerializer.Serialize(message);

            try
            {
                connection.Stream.Write(bytes, 0, bytes.Length);
                connection.Stream.WriteByte(0);
            }
            catch (Exception)
            {
                Close();

                throw;
            }
        }

        public void Close()
        {
            if (connection == null)
                return;
            connection.Dispose();
            connection = null;
        }
    }
}