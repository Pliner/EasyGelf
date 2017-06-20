using System;
using System.Threading;
using EasyGelf.Core;
using EasyGelf.Core.Transports;
using NUnit.Framework;

namespace EasyGelf.Tests.Core.Transport
{
    using System.Threading.Tasks;

    [TestFixture]
    public class BufferedTransportTests
    {
        private const int MessageCount = 100;

        [Test]
        public async Task ShouldSendMessage()
        {
            var countingTransport = new SuceedCountingTransport();
            var bufferedTransport = new BufferedTransport(new SilentLogger(), countingTransport);
            for (var i = 0; i < MessageCount; ++i)
            {
                var message = new GelfMessage();
                await bufferedTransport.Send(message);
            }
            bufferedTransport.Close();
            Assert.AreEqual(MessageCount, countingTransport.Count);
        }

        [Test]
        public async Task ShouldSkipMessageIfSendFailed()
        {
            var countingTransport = new FailCountingTransport();
            var bufferedTransport = new BufferedTransport(new SilentLogger(), countingTransport);
            for (var i = 0; i < MessageCount; ++i)
            {
                var message = new GelfMessage();
                await bufferedTransport.Send(message);
            }
            bufferedTransport.Close();
            Assert.AreEqual(MessageCount, countingTransport.Count);
        }

        private class SuceedCountingTransport : ITransport
        {
            private long count;

            public long Count {
                get { return Interlocked.Read(ref count); }
            }

            public async Task Send(GelfMessage message)
            {
                Interlocked.Increment(ref count);
            }

            public void Close()
            {
            }
        }

        private class FailCountingTransport : ITransport
        {
            private long count;

            public long Count
            {
                get { return Interlocked.Read(ref count); }
            }

            public async Task Send(GelfMessage message)
            {
                Interlocked.Increment(ref count);
                throw new Exception();
            }

            public void Close()
            {
            }
        }
    }
}