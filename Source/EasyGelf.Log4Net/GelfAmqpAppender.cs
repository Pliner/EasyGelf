using EasyGelf.Core;
using EasyGelf.Core.Amqp;
using EasyGelf.Core.Encoders;
using JetBrains.Annotations;

namespace EasyGelf.Log4Net
{
    public sealed class GelfAmqpAppender : GelfAppenderBase
    {
        private const int AmqpMessageSize = 50*1024;

        public GelfAmqpAppender()
        {
            Exchange = "Gelf";
            Queue = "Gelf";
            ExchangeType = "fanout";
            RoutingKey = "#";
        }

        [UsedImplicitly]
        public string ConnectionUri { get; set; }

        [UsedImplicitly]
        public string Exchange { get; set; }

        [UsedImplicitly]
        public string ExchangeType { get; set; }

        [UsedImplicitly]
        public string RoutingKey { get; set; }

        [UsedImplicitly]
        public string Queue { get; set; }

        protected override ITransport InitializeTransport()
        {
            var configuration = new AmqpTransportConfiguration
                {
                    ConnectionUri = ConnectionUri, 
                    Exchange = Exchange, 
                    ExchangeType = ExchangeType, 
                    Queue = Queue, 
                    RoutingKey = RoutingKey
                };
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(AmqpMessageSize));
            return new AmqpTransport(configuration, encoder);
        }
    }
}