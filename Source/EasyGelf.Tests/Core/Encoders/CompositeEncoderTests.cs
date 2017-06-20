using System.Collections.Generic;
using System.Text;
using EasyGelf.Core.Encoders;
using NUnit.Framework;
using System.Linq;

namespace EasyGelf.Tests.Core.Encoders
{
    using System.Threading.Tasks;

    using Moq;

    [TestFixture]
    public class CompositeEncoderTests
    {
        [Test]
        public async Task ShouldWorkWithZeroEncoders()
        {
            var compositeEncoder = new CompositeEncoder();
            var bytes = Encoding.UTF8.GetBytes("lalala");
            var encoderResult = (await compositeEncoder.Encode(bytes)).ToArray();
            Assert.AreEqual(1, encoderResult.Count());
            Assert.AreEqual(bytes, encoderResult.ElementAt(0));
        }

        [Test]
        public async Task ShouldWorkWithMultipleEncoders()
        {
            var firstEncoder = new Mock<ITransportEncoder>();
            var secondEncoder = new Mock<ITransportEncoder>();
            var compositeEncoder = new CompositeEncoder(firstEncoder.Object, secondEncoder.Object);
            var bytes = Encoding.UTF8.GetBytes("lalala");
            firstEncoder.Setup(x => x.Encode(bytes)).Returns(Task.FromResult<IEnumerable<byte[]>>(new List<byte[]> { bytes }));
            secondEncoder.Expect(x => x.Encode(bytes)).Returns(Task.FromResult <IEnumerable<byte[]>> (new List<byte[]> { bytes }));
            var encoderResult = (await compositeEncoder.Encode(bytes)).ToArray();
            Assert.AreEqual(1, encoderResult.Count());
            Assert.AreEqual(bytes, encoderResult.ElementAt(0));
        }
    }

}