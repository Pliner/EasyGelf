using JetBrains.Annotations;

namespace EasyGelf.Core
{
    public interface IIdGenerator
    {
        [NotNull]
        byte[] Generate([NotNull]byte[] message);
    }
}