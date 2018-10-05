using System;
using System.Threading;
using NLog;

namespace EasyGelf.NLog.Example
{
    public static class EntryPoint
    {
        private static readonly Logger Log = LogManager.GetLogger("ExampleLog");

        public static void Main()
        {
            var cancelationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) => cancelationTokenSource.Cancel();
            while (!cancelationTokenSource.IsCancellationRequested)
            {
                Log.Info("I'm alive");
                
                try
                {
                    ThrowException();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Descriptive message example");
                }
                
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        private static void ThrowException()
        {
            throw new Exception("Exception example");
        }
    }
}
