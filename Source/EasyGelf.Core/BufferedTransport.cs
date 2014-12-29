using System.Collections.Concurrent;
using System.Threading;

namespace EasyGelf.Core
{
    public class BufferedTransport : ITransport
    {
        private readonly BlockingCollection<byte[]> bytesToSend;
        private readonly CancellationTokenSource cancellationTokenSource;

        public BufferedTransport(ITransport transport)
        {
            cancellationTokenSource = new CancellationTokenSource();
            bytesToSend = new BlockingCollection<byte[]>();
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

        public void Send(byte[] bytes)
        {
            bytesToSend.Add(bytes, cancellationTokenSource.Token);
        }

        public void Close()
        {
            bytesToSend.CompleteAdding();
            cancellationTokenSource.Cancel();
        }
    }
}