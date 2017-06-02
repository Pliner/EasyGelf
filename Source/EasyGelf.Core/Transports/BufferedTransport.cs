﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace EasyGelf.Core.Transports
{
    public sealed class BufferedTransport : ITransport, IDisposable
    {
        private readonly BlockingCollection<GelfMessage> buffer = new BlockingCollection<GelfMessage>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEventSlim stopEvent = new ManualResetEventSlim(false);
        private readonly IEasyGelfLogger logger;
        private readonly ITransport transport;

        public BufferedTransport(IEasyGelfLogger logger, ITransport transport)
        {
            this.logger = logger;
            this.transport = transport;
            new Thread(PollAndSend)
            {
                IsBackground = true,
                Name = "EasyGelf Buffered Transport Thread"
            }.Start();
        }

        private void PollAndSend()
        {
            GelfMessage message;
            var cancellationToken = cancellationTokenSource.Token;
            try
            {
                while (buffer.TryTake(out message, -1, cancellationToken))
                {
                    SafeSendMessage(message);
                }
            }
            catch
            {
                while (buffer.TryTake(out message))
                {
                    SafeSendMessage(message);
                }
            }

            stopEvent.Set();
        }

        private void SafeSendMessage(GelfMessage mesage)
        {
            try
            {
                transport.Send(mesage);
            }
            catch (Exception exception)
            {
                logger.Error("Cannot send message", exception);
            }
        }

        public void Send(GelfMessage message)
        {
            buffer.Add(message, cancellationTokenSource.Token);
        }

        public void Close() => Task.Factory.StartNew(
            Dispose,
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default
        );

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            buffer.CompleteAdding();
            stopEvent.Wait();
            transport.Close();
            buffer.Dispose();
            cancellationTokenSource.Dispose();
            stopEvent.Dispose();
        }
    }
}