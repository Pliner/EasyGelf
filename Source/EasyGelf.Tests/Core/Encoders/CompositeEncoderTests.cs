using System.Text;
using EasyGelf.Core.Encoders;
using NUnit.Framework;
using System.Linq;

namespace EasyGelf.Tests.Core.Encoders
{
    [TestFixture]
    public class CompositeEncoderTests
    {
        [Test]
        public void ShouldWorkWithZeroEncoders()
        {
            var compositeEncoder = new CompositeEncoder();
            var bytes = Encoding.UTF8.GetBytes("lalala");
            var encoderResult = compositeEncoder.Encode(bytes).ToArray();
            Assert.AreEqual(1, encoderResult.Count());
            Assert.AreEqual(bytes, encoderResult.ElementAt(0));
        }
    }
}