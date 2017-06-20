using System;
using EasyGelf.Core;
using EasyGelf.Core.Transports;
using NUnit.Framework;

namespace EasyGelf.Tests.Core.Transport
{
    using System.Threading.Tasks;

    using Moq;

    [TestFixture]
    public class RetryTransportTests
    {
        private readonly int retryCount = 5;
        private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(50);
        private Mock<ITransport> mainTransport;
        private ITransport retryingTransport;


        [SetUp]
        public void SetUp()
        {
            mainTransport = new Mock<ITransport>();
            retryingTransport = new RetryingTransport(new SilentLogger(), mainTransport.Object, retryCount, retryDelay);
        }

        [Test]
        public void ShouldContinueWhenSendSuceed()
        {
            var message = new GelfMessage();
            mainTransport.Setup(x => x.Send(message));
            retryingTransport.Send(message);
        }

        [Test]
        public void ShouldSendAgainIfFirstAttempFailed()
        {
            var message = new GelfMessage();
            mainTransport.Setup(x => x.Send(message)).Throws(new Exception());
            mainTransport.Setup(x => x.Send(message));
            retryingTransport.Send(message);
        }

        [Test]
        public async Task ShouldFailIfAllAttemptsFailed()
        {
            try
            {
                var message = new GelfMessage();
                for (var i = 0; i < retryCount; ++i) mainTransport.Setup(x => x.Send(message)).Throws(new Exception());
                await retryingTransport.Send(message);
            }
            catch (Exception)
            {
                return;
            }

            throw new Exception();
    }
    }
}