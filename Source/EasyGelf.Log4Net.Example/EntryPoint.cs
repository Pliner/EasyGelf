using System;
using System.IO;
using System.Threading;
using log4net;
using log4net.Config;

namespace EasyGelf.Log4Net.Example
{
    public static class EntryPoint
    {
        private static readonly ILog Log = LogManager.GetLogger("EasyGelf.Log4Net", "ExampleLog");

        public static void Main()
        {
            ConfigureLogging();
            var cancelationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) => cancelationTokenSource.Cancel();
            while (!cancelationTokenSource.IsCancellationRequested)
            {
                Log.Info("I'm alive");
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        private static void ConfigureLogging()
        {
            var fileInfo = new FileInfo("log4net.config");
            if (!fileInfo.Exists)
                throw new Exception();
            var repository = LogManager.CreateRepository("EasyGelf.Log4Net");
            XmlConfigurator.Configure(repository, fileInfo);
        }
    }
}
