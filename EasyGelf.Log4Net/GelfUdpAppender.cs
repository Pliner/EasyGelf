using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using EasyGelf.Core;
using log4net.Appender;
using log4net.Core;

namespace EasyGelf.Log4Net
{
    public class GelfUdpAppender : AppenderSkeleton
    {
        private readonly IGelfMessageSerializer serializer = new GelfMessageSerializer();
        private ITransport transport;
        
        public GelfUdpAppender()
        {
            Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }
        public IPAddress RemoteAddress { get; set; }
        public int RemotePort { get; set; }

        protected override bool RequiresLayout
        {
            get { return true; }
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            try
            {
                var endpoint = new IPEndPoint(RemoteAddress, RemotePort);
                transport = new UdpTransport(new TransportConfiguration
                {
                    SplitLargeMessages = true,
                    LargeMessageSize = 1024,
                    MessageChunkSize = 1024,
                    Topology = new[] { endpoint }
                });
            }
            catch (Exception exception)
            {
                ErrorHandler.Error("Failed to create UdpTransport", exception);
                throw;
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (transport == null)
                return;
            transport.Close();
            transport = null;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                var renderedEvent = RenderLoggingEvent(loggingEvent);
                var gelfMessage = new GelfMessage
                    {
                        Level = loggingEvent.Level.ToGelf(),
                        Host = Environment.MachineName,
                        Timestamp = loggingEvent.TimeStamp,
                        FullMessage = renderedEvent,
                        ShortMessage = renderedEvent.Truncate(200),
                        AdditionalFields = new Dictionary<string, string>()
                    };
                var serializedGelfMessage = serializer.Serialize(gelfMessage);
                transport.Send(Encoding.GetBytes(serializedGelfMessage).GZip());
            }
            catch (Exception exception)
            {
                ErrorHandler.Error("Unable to send logging event to remote host " + RemoteAddress + " on port " + RemotePort + ".", exception, ErrorCode.WriteFailure);
            }
        }
    }
}