using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace EasyGelf.Core.Amqp
{
    public sealed class AmqpTransport : AbstractTransport
    {
        private readonly IAmqpTransportConfiguration configuration;
        private IConnection connection;
        private IModel channel;
        private bool isConnected;

        public AmqpTransport(IAmqpTransportConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
        }

        private void EnsureConnected()
        {
            if (isConnected)
                return;
            try
            {
                var connectionFactory = new ConnectionFactory
                    {
                        Uri = configuration.ConnectionUri,
                        AutomaticRecoveryEnabled = true,
                        TopologyRecoveryEnabled = true,
                        UseBackgroundThreadsForIO = true,
                        RequestedHeartbeat = 10,
                    };
                connection = connectionFactory.CreateConnection();
                channel = connection.CreateModel();
                channel.ExchangeDeclare(configuration.Exchange, configuration.ExchangeType, true);
                channel.QueueDeclare(configuration.Queue, true, false, false, new Dictionary<string, object>());
                channel.QueueBind(configuration.Queue, configuration.Exchange, configuration.RoutingKey);
                isConnected = true;
            }
            catch (Exception)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
                throw;
            }
        }

        protected override void SendInternal(byte[] bytes)
        {
            EnsureConnected();
            channel.BasicPublish(configuration.Exchange, configuration.RoutingKey, false, false, new BasicProperties{DeliveryMode = 1}, bytes);
        }

        public override void Close()
        {
            CoreExtentions.SafeDo(channel.Close);
            CoreExtentions.SafeDo(connection.Close);
        }
    }
}