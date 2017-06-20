﻿using System;
using System.Collections.Generic;
using EasyGelf.Core.Encoders;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace EasyGelf.Core.Transports.Amqp
{
    using System.Threading.Tasks;

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

        public async Task Send(GelfMessage message)
        {
            EstablishConnection();
            foreach (var bytes in await encoder.Encode(messageSerializer.Serialize(message)))
            {
                var basicProperties = new BasicProperties
                    {
                        DeliveryMode = configuration.Persistent ? (byte)2 : (byte)1
                    };
                channel.BasicPublish(configuration.Exchange, configuration.RoutingKey, false, basicProperties, bytes);
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