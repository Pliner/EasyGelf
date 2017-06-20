﻿using System;
using System.Threading;

namespace EasyGelf.Core.Transports
{
    using System.Threading.Tasks;

    public sealed class RetryingTransport : ITransport
    {
        private readonly IEasyGelfLogger logger;
        private readonly ITransport transport;
        private readonly int retryCount;
        private readonly TimeSpan retryDelay;

        public RetryingTransport(IEasyGelfLogger logger, ITransport transport, int retryCount, TimeSpan retryDelay)
        {
            this.logger = logger;
            this.transport = transport;
            this.retryCount = retryCount;
            this.retryDelay = retryDelay;
        }

        public async Task Send(GelfMessage message)
        {
            var sendRetryCount = retryCount;
            while (true)
            {
                try
                {
                    await this.transport.Send(message);
                    break;
                }
                catch(Exception exception)
                {
                    sendRetryCount--;
                    if(sendRetryCount <= 0)
                        throw;
                    Thread.Sleep(retryDelay);
                    logger.Error("Cannot send message. Retrying...", exception);
                }
            }
        }

        public void Close()
        {
            transport.Close();
        }
    }
}