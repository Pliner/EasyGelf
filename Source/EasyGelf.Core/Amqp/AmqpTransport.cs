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
        private IModel channel;
        private IConnection connection;

        public AmqpTransport(AmqpTransportConfiguration configuration, ITransportEncoder encoder, IGelfMessageSerializer messageSerializer)
        {
            this.configuration = configuration;
            this.encoder = encoder;
            this.messageSerializer = messageSerializer;
        }

        public void Send(GelfMessage message)
        {
            EstablishConnection();
            foreach (var bytes in encoder.Encode(messageSerializer.Serialize(message)))
                {
                    channel.BasicPublish(configuration.Exchange, configuration.RoutingKey, false, false, new BasicProperties {DeliveryMode = 1}, bytes);
                }
         }

        public void Close()
        {
            channel.SafeDispose();
            channel = null;
            connection.SafeDispose();
            connection = null;
        }

        private void EstablishConnection()
        {
            try
            {
                if (connection != null)
                {
                    if (connection.IsOpen)
                        return;
                    throw new CannotConnectException(string.Format("Cannot connect to {0}", configuration.ConnectionUri));
                }

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
            }
            catch (Exception exception)
            {
                channel.SafeDispose();
                channel = null;
                connection.SafeDispose();
                connection = null;
                throw new CannotConnectException(string.Format("Cannot connect to {0}", configuration.ConnectionUri), exception);
            }
        }
    }
}