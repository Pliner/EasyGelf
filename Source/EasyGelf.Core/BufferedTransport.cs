using System.Collections.Concurrent;
using System.Threading;

namespace EasyGelf.Core
{
    public sealed class BufferedTransport : ITransport
    {
        private readonly BlockingCollection<GelfMessage> buffer = new BlockingCollection<GelfMessage>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        
        public BufferedTransport(ITransport transport)
        {
            new Thread(() =>
                {
                    var cancellationToken = cancellationTokenSource.Token;
                    try
                    {
                        GelfMessage mesage;
                        while (buffer.TryTake(out mesage, -1, cancellationToken))
                        {
                            try
                            {
                                transport.Send(mesage);
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                        GelfMessage message;
                        while (buffer.TryTake(out message))
                        {
                            try
                            {
                                transport.Send(message);
                            }
                            catch
                            {
                            }
                        }
                    }
                    transport.Close();
                }) {IsBackground = true, Name = "EasyGelf Buffered Transport Thread"}.Start();
        }

        public void Send(GelfMessage message)
        {
            buffer.Add(message, cancellationTokenSource.Token);
        }

        public void Close()
        {
            buffer.CompleteAdding();
            cancellationTokenSource.Cancel();
        }
    }
}