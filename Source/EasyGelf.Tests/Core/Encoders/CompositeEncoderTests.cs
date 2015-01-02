using System.Collections.Generic;
using System.Text;
using EasyGelf.Core.Encoders;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace EasyGelf.Tests.Core.Encoders
{
    [TestFixture]
    public class CompositeEncoderTests
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void ShouldWorkWithZeroEncoders()
        {
            var compositeEncoder = new CompositeEncoder();
            var bytes = Encoding.UTF8.GetBytes("lalala");
            var encoderResult = compositeEncoder.Encode(bytes).ToArray();
            Assert.AreEqual(1, encoderResult.Count());
            Assert.AreEqual(bytes, encoderResult.ElementAt(0));
        }

        [Test]
        public void ShouldWorkWithMultipleEncoders()
        {
            var firstEncoder = mockRepository.StrictMultiMock<ITransportEncoder>();
            firstEncoder.Replay();
            var secondEncoder = mockRepository.StrictMultiMock<ITransportEncoder>();
            secondEncoder.Replay();
            var compositeEncoder = new CompositeEncoder(firstEncoder, secondEncoder);
            var bytes = Encoding.UTF8.GetBytes("lalala");
            firstEncoder.Expect(x => x.Encode(bytes)).Return(new List<byte[]> { bytes });
            secondEncoder.Expect(x => x.Encode(bytes)).Return(new List<byte[]> { bytes });
            var encoderResult = compositeEncoder.Encode(bytes).ToArray();
            Assert.AreEqual(1, encoderResult.Count());
            Assert.AreEqual(bytes, encoderResult.ElementAt(0));
        }

        [TearDown]
        public void TearDown()
        {
            mockRepository.VerifyAll();
        }
    }

}