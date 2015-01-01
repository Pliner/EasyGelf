using System.Collections.Generic;
using System.Linq;

namespace EasyGelf.Core.Encoders
{
    public sealed class CompositeEncoder : ITransportEncoder
    {
        private readonly ITransportEncoder[] encoders;

        public CompositeEncoder(params ITransportEncoder[] encoders)
        {
            this.encoders = encoders;
        }

        public IEnumerable<byte[]> Encode(byte[] bytes)
        {
            var seed = (IEnumerable<byte[]>) new List<byte[]> {bytes};
            return encoders.Aggregate(seed, (current, encoder) => current.SelectMany(encoder.Encode));
        }
    }
}