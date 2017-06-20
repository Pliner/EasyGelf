using System.Collections.Generic;
using System.Linq;

namespace EasyGelf.Core.Encoders
{
    using System.Threading.Tasks;

    public sealed class CompositeEncoder : ITransportEncoder
    {
        private readonly ITransportEncoder[] encoders;

        public CompositeEncoder(params ITransportEncoder[] encoders)
        {
            this.encoders = encoders;
        }

        public async Task<IEnumerable<byte[]>> Encode(byte[] bytes)
        {

            return encoders.Aggregate(
                (IEnumerable<byte[]>)new List<byte[]> { bytes },
                (current, encoder) => current.SelectMany(x => encoder.Encode(x).Result));
        }
    }
}