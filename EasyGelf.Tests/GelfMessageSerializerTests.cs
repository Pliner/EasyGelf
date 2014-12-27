using System;
using EasyGelf.Core;
using NUnit.Framework;

namespace EasyGelf.Tests
{
    [TestFixture]
    public class GelfMessageSerializerTests
    {
        private IGelfMessageSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer = new GelfMessageSerializer();
        }

        [Test]
        public void TestSimple()
        {
            var dateTime = new DateTime(2015, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var message = new GelfMessage
                {
                    Host = "example.org", 
                    ShortMessage = "A short message that helps you identify what is going on", 
                    FullMessage = "Backtrace here\n\nmore stuff",
                    Timestamp = dateTime, 
                    Level = GelfLevel.Alert,
                };
            var serializedMessage = serializer.Serialize(message);
            Assert.AreEqual("{\"version\":\"1.1\",\"host\":\"example.org\",\"short_message\":\"A short message that helps you identify what is going on\",\"full_message\":\"Backtrace here\\n\\nmore stuff\",\"timestamp\":1420070400.0,\"level\":1}", serializedMessage);
        }
    }
}
