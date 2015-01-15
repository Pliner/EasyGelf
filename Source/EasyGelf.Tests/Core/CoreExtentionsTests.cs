using NUnit.Framework;
using EasyGelf.Core;

namespace EasyGelf.Tests.Core
{
    [TestFixture]
    public class CoreExtentionsTests
    {
        [Test]
        public void TestUdpMessageSize()
        {
            Assert.AreEqual(512, 511.UdpMessageSize());
            Assert.AreEqual(512, 512.UdpMessageSize());
            Assert.AreEqual(8192, 8192.UdpMessageSize());
            Assert.AreEqual(8192, 8193.UdpMessageSize());
        }
    }
}