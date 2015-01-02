using System.Linq;
using EasyGelf.Core.Encoders;
using NUnit.Framework;

namespace EasyGelf.Tests.Core.Encoders
{
    [TestFixture]
    public class ChunkingEncoderTests
    {
        private const int MaxSize = 32;
        private ChunkingEncoder chunkingEncoder;

        [SetUp]
        public void SetUp()
        {
            chunkingEncoder = new ChunkingEncoder(new MessageBasedIdGenerator(), MaxSize);
        }

        [Test]
        public void ShouldEncodeMessageLessThanMaxSize()
        {
            var bytes = new byte[32];
            for (var i = 0; i < bytes.Length; ++i)
                bytes[i] = (byte) i;
            var encodeResult = chunkingEncoder.Encode(bytes).ToArray();
            Assert.AreEqual(1, encodeResult.Count());
            Assert.AreEqual(bytes, encodeResult.ElementAt(0));
        }


        [Test]
        public void ShouldEncodeMessageGreaterThanMaxSize()
        {
            var bytes = new byte[39];
            for (var i = 0; i < bytes.Length; ++i)
                bytes[i] = (byte)i;
            var encodeResult = chunkingEncoder.Encode(bytes).ToArray();
            Assert.AreEqual(2, encodeResult.Count());
            var firstChunk = encodeResult.ElementAt(0);
            var secondChunk = encodeResult.ElementAt(1);
            Assert.AreEqual(bytes.Take(20).ToArray(), firstChunk.Skip(12).ToArray());
            Assert.AreEqual(bytes.Skip(20).ToArray(), secondChunk.Skip(12).ToArray());
        }
    }
}