using System;
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
        public string Facility { get; set; }

        protected GelfAppenderBase()
        {
            Facility = "gelf";
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
                var messageBuilder = new GelfMessageBuilder(renderedEvent, Environment.MachineName)
                    .SetLevel(loggingEvent.Level.ToGelf())
                    .SetTimestamp(loggingEvent.TimeStamp)
                    .SetAdditionalField("facility", Facility)
                    .SetAdditionalField("loggerName", loggingEvent.LoggerName)
                    .SetAdditionalField("threadName", loggingEvent.ThreadName);

                transport.Send(messageBuilder.ToMessage());
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