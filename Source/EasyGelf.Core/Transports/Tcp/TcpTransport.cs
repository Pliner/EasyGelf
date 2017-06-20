using System;

namespace EasyGelf.Core.Transports.Tcp
{
    using System.Threading.Tasks;

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


        private async Task EstablishConnectionAsync()
        {
            if (connection != null)
            {
                return;
            }

            try
            {
                connection = createConnection();
                await connection.Open();
            }
            catch (Exception exception)
            {
                Close();

                throw new CannotConnectException(string.Format("Cannot connect to {0}", configuration.GetHost()), exception);
            }
        }


        public async Task Send(GelfMessage message)
        {
            await this.EstablishConnectionAsync();
            
            var bytes = messageSerializer.Serialize(message);

            try
            {
                await connection.Stream.WriteAsync(bytes, 0, bytes.Length);
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