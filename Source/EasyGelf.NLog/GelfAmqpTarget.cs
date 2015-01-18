using EasyGelf.Core;
using EasyGelf.Core.Amqp;
using EasyGelf.Core.Encoders;
using JetBrains.Annotations;
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

        [UsedImplicitly]
        public int MessageSize { get; set; }

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