using System;
using EasyGelf.Core;
using JetBrains.Annotations;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace EasyGelf.NLog
{
    public abstract class GelfTargetBase : TargetWithLayout
    {
        private ITransport transport;

        [UsedImplicitly]
        public string Facility { get; set; }

        [UsedImplicitly]
        public string HostName { get; set; }

        [UsedImplicitly]
        public bool UseBuffering { get; set; }

        protected GelfTargetBase()
        {
            Facility = "gelf";
            HostName = Environment.MachineName;
            UseBuffering = true;
        }

        protected abstract ITransport InitializeTransport();

        public void WriteLogEventInfo(LogEventInfo loggingEvent)
        {
            try
            {
                var renderedEvent = Layout.Render(loggingEvent);
                var messageBuilder = new GelfMessageBuilder(renderedEvent, HostName)
                    .SetLevel(ToGelf(loggingEvent.Level))
                    .SetTimestamp(loggingEvent.TimeStamp)
                    .SetAdditionalField("facility", Facility)
                    .SetAdditionalField("loggerName", loggingEvent.LoggerName);
               
                transport.Send(messageBuilder.ToMessage());
            }
            catch (Exception exception)
            {
                InternalLogger.Error("Failed to send message", exception);
            }
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            try
            {
                var initializedTransport = InitializeTransport();
                transport = UseBuffering ? new BufferedTransport(initializedTransport) : initializedTransport;
            }
            catch (Exception exception)
            {
                InternalLogger.Error("Failed to initialize transport", exception);
                throw;
            }
        }

        protected override void CloseTarget()
        {
            base.CloseTarget();

            if (transport == null)
                return;
            CoreExtentions.SafeDo(transport.Close);
            transport = null;
        }


        private GelfLevel ToGelf(LogLevel level)
        {
            if (level == LogLevel.Debug)
            {
                return GelfLevel.Debug;
            }
            if (level == LogLevel.Fatal)
            {
                return GelfLevel.Critical;
            }
            if (level == LogLevel.Info)
            {
                return GelfLevel.Informational;
            }
            if (level == LogLevel.Trace)
            {
                return GelfLevel.Informational;
            }
            if (level == LogLevel.Warn)
            {
                return GelfLevel.Warning;
            }
            return GelfLevel.Error;
        }
    }
}
