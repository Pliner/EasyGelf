using System;
using System.Collections.Generic;
using System.Text;
using EasyGelf.Core;
using JetBrains.Annotations;
using log4net.Appender;
using log4net.Core;

namespace EasyGelf.Log4Net
{
    public abstract class GelfAppenderBase  : AppenderSkeleton
    {
        private ITransport transport;

        [UsedImplicitly]
        public Encoding Encoding { get; set; }

        protected GelfAppenderBase()
        {
            Encoding = Encoding.UTF8;
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            try
            {
                transport = InitializeTransport();
            }
            catch (Exception exception)
            {
                ErrorHandler.Error("Failed to create UdpTransport", exception);
                throw;
            }
        }

        protected override bool RequiresLayout
        {
            get { return true; }
        }

        protected abstract ITransport InitializeTransport();

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                var renderedEvent = RenderLoggingEvent(loggingEvent);
                var message = new GelfMessage
                {
                    Level = loggingEvent.Level.ToGelf(),
                    Host = Environment.MachineName,
                    Timestamp = loggingEvent.TimeStamp,
                    FullMessage = renderedEvent,
                    ShortMessage = renderedEvent.Truncate(200),
                    AdditionalFields = new Dictionary<string, string>()
                };
                transport.Send(message);
            }
            catch (Exception exception)
            {
                ErrorHandler.Error("Unable to send logging event to remote host", exception, ErrorCode.WriteFailure);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (transport == null)
                return;
            CoreExtentions.SafeDo(transport.Close);
            transport = null;
        }

    }
}