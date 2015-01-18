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
            ConnectionUri = "amqp://";
            Persistent = true;
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

        [UsedImplicitly]
        public bool Persistent { get; set; }

        protected override ITransport InitializeTransport()
        {
            var configuration = new AmqpTransportConfiguration
                {
                    ConnectionUri = ConnectionUri, 
                    Exchange = Exchange, 
                    ExchangeType = ExchangeType, 
                    Queue = Queue, 
                    RoutingKey = RoutingKey,
                    Persistent = Persistent,
                };
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(new MessageBasedIdGenerator(), AmqpMessageSize));
            return new AmqpTransport(configuration, encoder, new GelfMessageSerializer());
        }
    }
}