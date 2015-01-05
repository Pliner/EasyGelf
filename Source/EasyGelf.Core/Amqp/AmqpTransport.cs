using System;
using System.Collections.Generic;
using EasyGelf.Core.Encoders;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace EasyGelf.Core.Amqp
{
    public sealed class AmqpTransport : ITransport
    {
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
                if (connection == null)
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
                    return true;
                }
                return connection.IsOpen;
            }
            catch
            {
                channel.SafeDispose();
                channel = null;
                connection.SafeDispose();
                connection = null;
                return false;
            }
        }

        public void Send(GelfMessage message)
        {
            if (TryRestoreConnection())
            {
                foreach (var bytes in encoder.Encode(messageSerializer.Serialize(message)))
                {
                    channel.BasicPublish(configuration.Exchange, configuration.RoutingKey, false, false, new BasicProperties {DeliveryMode = 1}, bytes);
                }
            }
            else
                throw new InvalidOperationException();
        }

        public void Close()
        {
            channel.SafeDispose();
            channel = null;
            connection.SafeDispose();
            connection = null;
        }
    }
}