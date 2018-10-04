using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Transports;
using EasyGelf.Core.Transports.Amqp;

namespace EasyGelf.Log4Net
{
    public sealed class GelfAmqpAppender : GelfAppenderBase
    {
        public GelfAmqpAppender()
        {
            Exchange = "Gelf";
            Queue = "Gelf";
            ExchangeType = "fanout";
            RoutingKey = "#";
            ConnectionUri = "amqp://";
            Persistent = true;
            MessageSize = 50*1024;
        }

        public int MessageSize { get; set; }

        public string ConnectionUri { get; set; }

        public string Exchange { get; set; }

        public string ExchangeType { get; set; }

        public string RoutingKey { get; set; }

        public string Queue { get; set; }

        public bool Persistent { get; set; }

        protected override ITransport InitializeTransport(IEasyGelfLogger logger)
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
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(new MessageBasedIdGenerator(), MessageSize));
            return new AmqpTransport(configuration, encoder, new GelfMessageSerializer(logger));
        }
    }
}