using System;
using System.Collections.Generic;
using System.Text;
using EasyGelf.Core.Encoders;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace EasyGelf.Core.Amqp
{
    public sealed class AmqpTransport : ITransport
    {
        private readonly object lockery = new object();

        private readonly AmqpTransportConfiguration configuration;
        private readonly ITransportEncoder encoder;
        private readonly IGelfMessageSerializer messageSerializer;
        private IConnection connection;
        private IModel channel;

        public AmqpTransport(AmqpTransportConfiguration configuration, ITransportEncoder encoder, IGelfMessageSerializer messageSerializer)
        {
            this.configuration = configuration;
            this.encoder = encoder;
            this.messageSerializer = messageSerializer;
        }

        private bool TryRestoreConnection()
        {
            try
            {
                if (connection != null)
                    return true;
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
                return true;
            }
            catch (Exception)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
                return false;
            }  
        }

        public void Send(GelfMessage message)
        {
            lock (lockery)
            {
                if (TryRestoreConnection())
                {
                    foreach (var bytes in encoder.Encode(messageSerializer.Serialize(message)))
                    {
                        channel.BasicPublish(configuration.Exchange, configuration.RoutingKey, false, false,
                                             new BasicProperties {DeliveryMode = 1}, bytes);
                    }
                }
            }
        }

        public void Close()
        {
            lock (lockery)
            {
                CoreExtentions.SafeDo(channel.Close);
                CoreExtentions.SafeDo(connection.Close);
            }
        }
    }
}