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
        private readonly IAmqpTransportConfiguration configuration;
        private readonly ITransportEncoder encoder;
        private IConnection connection;
        private IModel channel;

        public AmqpTransport(IAmqpTransportConfiguration configuration, ITransportEncoder encoder)
        {
            this.configuration = configuration;
            this.encoder = encoder;
        }

        private bool IsConnectionDone()
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
            if (!IsConnectionDone())
                return;
            foreach (var bytes in encoder.Encode(Encoding.UTF8.GetBytes(message.Serialize())))
            {
                channel.BasicPublish(configuration.Exchange, configuration.RoutingKey, false, false, new BasicProperties {DeliveryMode = 1}, bytes);
            }
        }

        public void Close()
        {
            CoreExtentions.SafeDo(channel.Close);
            CoreExtentions.SafeDo(connection.Close);
        }
    }
}