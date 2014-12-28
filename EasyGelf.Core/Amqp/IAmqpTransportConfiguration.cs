using JetBrains.Annotations;

namespace EasyGelf.Core.Amqp
{
    public interface IAmqpTransportConfiguration : IAbstractTransportConfiguration
    {
        [NotNull]
        string ConnectionUri { get; }

        [NotNull]
        string Exchange { get; }

        [NotNull]
        string ExchangeType { get; }

        [NotNull]
        string RoutingKey { get; }

        [NotNull]
        string Queue { get; }
    }
}