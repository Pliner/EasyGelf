using EasyGelf.Core;
using EasyGelf.Core.Amqp;
using JetBrains.Annotations;

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
            return new BufferedTransport(new AmqpTransport(new AmqpTransportConfiguration
                {
                    ConnectionUri = ConnectionUri,
                    Exchange = Exchange,
                    ExchangeType = ExchangeType,
                    Queue = Queue,
                    RoutingKey = RoutingKey,
                    SplitLargeMessages = false,
                }));
        }
    }
}