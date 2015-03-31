namespace EasyGelf.Core.Transports.Amqp
{
    public sealed class AmqpTransportConfiguration
    {
        public string ConnectionUri { get; set; }

        public string Exchange { get; set; }

        public string ExchangeType { get; set; }

        public string RoutingKey { get; set; }

        public string Queue { get; set; }

        public bool Persistent { get; set; }
    }
}