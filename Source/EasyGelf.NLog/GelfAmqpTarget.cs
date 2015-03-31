using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Transports;
using EasyGelf.Core.Transports.Amqp;
using NLog.Targets;

namespace EasyGelf.NLog
{
    [Target("GelfAmqp")]
    public sealed class GelfAmqpTarget : GelfTargetBase
    {
        public GelfAmqpTarget()
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
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(new MessageBasedIdGenerator(), MessageSize));
            var configuration = new AmqpTransportConfiguration
                {
                    ConnectionUri = ConnectionUri,
                    Exchange = Exchange,
                    ExchangeType = ExchangeType,
                    Queue = Queue,
                    RoutingKey = RoutingKey,
                    Persistent = Persistent
                };
            return new AmqpTransport(configuration, encoder, new GelfMessageSerializer());
        }
    }
}