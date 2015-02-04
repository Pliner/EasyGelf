using System.Collections.Generic;

namespace EasyGelf.Core.Encoders
{
    public interface ITransportEncoder
    {
        IEnumerable<byte[]> Encode(byte[] bytes);
    }
}