using System;
using System.Threading;

namespace EasyGelf.Core
{
    public class RetryingTransport : ITransport
    {
        private readonly ITransport transport;
        private readonly int retryCount;
        private readonly TimeSpan retryDelay;

        public RetryingTransport(ITransport transport, int retryCount, TimeSpan retryDelay)
        {
            this.transport = transport;
            this.retryCount = retryCount;
            this.retryDelay = retryDelay;
        }

        public void Send(GelfMessage message)
        {
            var sendRetryCount = retryCount;
            while (true)
            {
                try
                {
                    transport.Send(message);
                }
                catch(Exception)
                {
                    if(sendRetryCount <= 0)
                        throw;
                    Thread.Sleep(retryDelay);
                    sendRetryCount--;
                }
            }
        }

        public void Close()
        {
            transport.Close();
        }
    }
}