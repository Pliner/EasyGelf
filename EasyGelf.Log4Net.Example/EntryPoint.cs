using System;
using System.IO;
using System.Threading;
using log4net;
using log4net.Config;

namespace EasyGelf.Log4Net.Example
{
    public static class EntryPoint
    {
        private static readonly ILog log = LogManager.GetLogger("ExampleLog");

        public static void Main(string[] args)
        {
            ConfigureLogging();
            var isRunning = true;
            Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    isRunning = false;
                };
            while (isRunning)
            {
                log.Info("I'm alive");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private static void ConfigureLogging()
        {
            var fileInfo = new FileInfo("log4net.config");
            if (!fileInfo.Exists)
                throw new Exception();
            XmlConfigurator.Configure(fileInfo);
        }
    }
}
