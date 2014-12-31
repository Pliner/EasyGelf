namespace EasyGelf.Core.Amqp
{
    public sealed class AmqpTransportConfiguration : IAmqpTransportConfiguration
    {
        public string ConnectionUri { get; set; }
        public string Exchange { get; set; }
        public string ExchangeType { get; set; }
        public string RoutingKey { get; set; }
        public string Queue { get; set; }
        public int MaxMessageSize { get; set; }
    }
}