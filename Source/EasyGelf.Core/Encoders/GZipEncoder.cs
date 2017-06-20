using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace EasyGelf.Core.Encoders
{
    using System.Threading.Tasks;

    public sealed class GZipEncoder : ITransportEncoder
    {
        public async Task<IEnumerable<byte[]>> Encode(byte[] bytes)
        {
            var result = new List<byte[]>();
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var compressed = new GZipStream(output, CompressionMode.Compress))
                    CopyTo(input, compressed);
                result.Add(output.ToArray());
            }

            return result;
        }

        private static void CopyTo(Stream source, Stream destination)
        {
            var buffer = new byte[16 * 1024];
            int bytesRead;

            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, bytesRead);
            }
        }
    }
}