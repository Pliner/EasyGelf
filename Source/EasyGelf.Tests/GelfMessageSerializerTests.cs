using System;
using System.Text;
using EasyGelf.Core;
using NUnit.Framework;

namespace EasyGelf.Tests
{
    [TestFixture]
    public class GelfMessageSerializerTests
    {
        private GelfMessageSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer = new GelfMessageSerializer();
        }

        [Test]
        public void ShouldSerializerSimpleMessage()
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
            var serializedMessage = Encoding.UTF8.GetString(serializer.Serialize(message));
            Assert.AreEqual("{\"version\":\"1.1\"," +
                            "\"host\":\"example.org\"," +
                            "\"short_message\":\"A short message that helps you identify what is going on\"," +
                            "\"full_message\":\"Backtrace here\\n\\nmore stuff\"," +
                            "\"timestamp\":1420070400," +
                            "\"level\":1}", serializedMessage);
        }

        [Test]
        public void ShouldSerializeMessageWithAdditionalFields()
        {
            var dateTime = new DateTime(2015, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var message = new GelfMessageBuilder("Backtrace here\n\nmore stuff", "example.org", dateTime, GelfLevel.Alert)
                .SetAdditionalField(GelfAdditionalFields.Facility, "facility")
                .SetAdditionalField(GelfAdditionalFields.LoggerName, "loggerName")
                .SetAdditionalField(GelfAdditionalFields.ThreadName, "threadName")
                .SetAdditionalField(GelfAdditionalFields.SourceFileName, "sourceFileName")
                .SetAdditionalField(GelfAdditionalFields.SourceLineNumber, "sourceLineNumber")
                .SetAdditionalField(GelfAdditionalFields.SourceClassName, "sourceClassName")
                .SetAdditionalField(GelfAdditionalFields.SourceMethodName, "sourceMethodName")
                .ToMessage();

            var serializedMessage = Encoding.UTF8.GetString(serializer.Serialize(message));
            Assert.AreEqual("{\"version\":\"1.1\"," +
                            "\"host\":\"example.org\"," +
                            "\"short_message\":\"Backtrace here\\n\\nmore stuff\"," +
                            "\"full_message\":\"Backtrace here\\n\\nmore stuff\"," +
                            "\"timestamp\":1420070400," +
                            "\"level\":1," +
                            "\"_facility\":\"facility\"," +
                            "\"_loggerName\":\"loggerName\"," +
                            "\"_threadName\":\"threadName\"," +
                            "\"_sourceFileName\":\"sourceFileName\"," +
                            "\"_sourceLineNumber\":\"sourceLineNumber\"," +
                            "\"_sourceClassName\":\"sourceClassName\"," +
                            "\"_sourceMethodName\":\"sourceMethodName\"}", serializedMessage);
        }
    }
}
