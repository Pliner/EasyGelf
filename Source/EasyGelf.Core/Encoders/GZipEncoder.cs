using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace EasyGelf.Core.Encoders
{
    public class GZipEncoder : ITransportEncoder
    {
        public IEnumerable<byte[]> Encode(byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var compressed = new GZipStream(output, CompressionMode.Compress))
                    input.CopyTo(compressed);
                yield return output.ToArray();
            }
        }
    }
}