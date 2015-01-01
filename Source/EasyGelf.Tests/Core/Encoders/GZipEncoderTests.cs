using System.IO;
using System.IO.Compression;
using System.Text;
using EasyGelf.Core.Encoders;
using NUnit.Framework;
using System.Linq;

namespace EasyGelf.Tests.Core.Encoders
{
    [TestFixture]
    public class GZipEncoderTests
    {
        [Test]
        public void ShoudUnzip()
        {
            var encoder = new GZipEncoder();
            var bytes = Encoding.UTF8.GetBytes("lalala");
            var encodeResult = encoder.Encode(bytes).ToArray();
            Assert.AreEqual(1, encodeResult.Count());
            var encodedBytes = encodeResult.ElementAt(0);
            Assert.AreEqual(bytes, Unzip(encodedBytes));
        }

        private static byte[] Unzip(byte[] bytes)
        {
            using (var output = new MemoryStream())
            {
                using (var input = new MemoryStream(bytes))
                using (var gzipInput = new GZipStream(input, CompressionMode.Decompress))
                {
                    gzipInput.CopyTo(output);
                }
                return output.ToArray();
            }
        }
    }
}