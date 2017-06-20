using System.Collections.Generic;

namespace EasyGelf.Core.Encoders
{
    using System.Threading.Tasks;

    public interface ITransportEncoder
    {
        Task<IEnumerable<byte[]>> Encode(byte[] bytes);
    }
}