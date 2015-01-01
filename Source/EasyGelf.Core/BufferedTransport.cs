using System.Collections.Concurrent;
using System.Threading;

namespace EasyGelf.Core
{
    public sealed class BufferedTransport : ITransport
    {
        private readonly BlockingCollection<GelfMessage> bytesToSend = new BlockingCollection<GelfMessage>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public BufferedTransport(ITransport transport)
        {
            new Thread(() =>
                {
                    foreach (var bytes in bytesToSend.GetConsumingEnumerable(cancellationTokenSource.Token))
                    {
                        try
                        {
                            transport.Send(bytes);
                        }
                        catch
                        {
                        }
                    }
                    CoreExtentions.SafeDo(transport.Close);
                }) {IsBackground = true, Name = "EasyGelf Buffered Transport Thread"}.Start();
        }

        public void Send(GelfMessage message)
        {
            bytesToSend.Add(message, cancellationTokenSource.Token);
        }

        public void Close()
        {
            bytesToSend.CompleteAdding();
            cancellationTokenSource.Cancel();
        }
    }
}