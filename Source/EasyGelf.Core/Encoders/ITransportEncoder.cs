using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyGelf.Core.Encoders
{
    public interface ITransportEncoder
    {
        [NotNull]
        IEnumerable<byte[]> Encode([NotNull]byte[] bytes);
    }
}