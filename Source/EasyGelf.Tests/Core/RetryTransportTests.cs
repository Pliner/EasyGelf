using System;
using EasyGelf.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace EasyGelf.Tests.Core
{
    [TestFixture]
    public class RetryTransportTests
    {
        private readonly int retryCount = 5;
        private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(50);
        private MockRepository mockRepository;
        private ITransport mainTransport;
        private ITransport retryingTransport;


        [SetUp]
        public void SetUp()
        {              
            mockRepository = new MockRepository();
            mainTransport = mockRepository.StrictMultiMock<ITransport>();
            mainTransport.Replay();
            retryingTransport = new RetryingTransport(mainTransport, retryCount, retryDelay);
        }

        [Test]
        public void ShouldContinueWhenSendSuceed()
        {
            var message = new GelfMessage();
            mainTransport.Expect(x => x.Send(message)).TentativeReturn();
            retryingTransport.Send(message);
        }

        [Test]
        public void ShouldSendAgainIfFirstAttempFailed()
        {
            var message = new GelfMessage();
            mainTransport.Expect(x => x.Send(message)).Throw(new Exception());
            mainTransport.Expect(x => x.Send(message)).TentativeReturn();
            retryingTransport.Send(message);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void ShouldFailIfAllAttemptsFailed()
        {
            var message = new GelfMessage();
            for(var i = 0; i < retryCount; ++i)
                mainTransport.Expect(x => x.Send(message)).Throw(new Exception());
            retryingTransport.Send(message);
        }

        [TearDown]
        public void TearDown()
        {
            mockRepository.VerifyAll();
        }
    }
}