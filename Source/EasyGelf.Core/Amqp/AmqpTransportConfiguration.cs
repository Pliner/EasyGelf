using System;
using JetBrains.Annotations;

namespace EasyGelf.Core.Amqp
{
    public sealed class AmqpTransportConfiguration
    {
        public TimeSpan ReconnectionTimeout { get; set; }

        [NotNull]
        public string ConnectionUri { get; set; }

        [NotNull]
        public string Exchange { get; set; }

        [NotNull]
        public string ExchangeType { get; set; }

        [NotNull]
        public string RoutingKey { get; set; }

        [NotNull]
        public string Queue { get; set; }
    }
}