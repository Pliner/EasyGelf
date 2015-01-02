using JetBrains.Annotations;

namespace EasyGelf.Core.Encoders
{
    public interface IChunkedMessageIdGenerator
    {
        [NotNull]
        byte[] GenerateId([NotNull]byte[] message);
    }
}