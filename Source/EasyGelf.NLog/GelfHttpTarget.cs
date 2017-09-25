using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyGelf.Core;
using EasyGelf.Core.Transports;
using EasyGelf.Core.Transports.Http;

namespace EasyGelf.NLog
{
    [Target("GelfHttp")]
    public sealed class GelfHttpTarget : GelfTargetBase
    {
        public string Url { get; set; }

        public int Timeout { get; set; }

        public int RemotePort { get; set; }

        public GelfHttpTarget()
        {
            Url = "http://localhost";
            Timeout = 3000;
            RemotePort = 80;
        }

        protected override ITransport InitializeTransport(IEasyGelfLogger logger)
        {
            var configuration = new HttpTransportConfiguration
            {
                Uri = Url,
                Timeout = 3000,
                Port = RemotePort
            };

            return new HttpTransport(configuration, new GelfMessageSerializer());
        }
    }
}
