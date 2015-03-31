using System;
using System.Globalization;
using EasyGelf.Core;
using EasyGelf.Core.Transports;
using NLog;
using NLog.Targets;

namespace EasyGelf.NLog
{
    public abstract class GelfTargetBase : TargetWithLayout
    {
        private ITransport transport;
        private IEasyGelfLogger logger;

        public string Facility { get; set; }

        public string HostName { get; set; }

        public bool IncludeSource { get; set; }
       
        public bool UseRetry { get; set; }

        public int RetryCount { get; set; }

        public TimeSpan RetryDelay { get; set; }

        public bool IncludeStackTrace { get; set; }

        public bool Verbose { get; set; }

        protected GelfTargetBase()
        {
            Facility = "gelf";
            HostName = Environment.MachineName;
            IncludeSource = true;
            UseRetry = true;
            RetryCount = 5;
            RetryDelay = TimeSpan.FromMilliseconds(50);
            IncludeStackTrace = true;
            Verbose = false;
        }

        protected abstract ITransport InitializeTransport(IEasyGelfLogger logger);

        protected override void Write(LogEventInfo loggingEvent)
        {
            try
            {
                var renderedEvent = Layout.Render(loggingEvent);
                var messageBuilder = new GelfMessageBuilder(renderedEvent, HostName, loggingEvent.TimeStamp, ToGelf(loggingEvent.Level))
                    .SetAdditionalField(GelfAdditionalFields.Facility, Facility)
                    .SetAdditionalField(GelfAdditionalFields.LoggerName, loggingEvent.LoggerName);
                if (IncludeSource)
                {
                    var userStackFrame = loggingEvent.UserStackFrame;
                    if (userStackFrame != null)
                    {
                        messageBuilder.SetAdditionalField(GelfAdditionalFields.SourceFileName, userStackFrame.GetFileName());
                        messageBuilder.SetAdditionalField(GelfAdditionalFields.SourceLineNumber, userStackFrame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture));
                    }
                }
                if (IncludeStackTrace)
                {
                    var exception = loggingEvent.Exception;
                    if (exception != null)
                    {
                        messageBuilder.SetAdditionalField(GelfAdditionalFields.ExceptionMessage, exception.Message);
                        messageBuilder.SetAdditionalField(GelfAdditionalFields.ExceptionStackTrace, exception.StackTrace);
                    }
                }
                transport.Send(messageBuilder.ToMessage());
            }
            catch (Exception exception)
            {
                logger.Error("Failed to send message", exception);
            }
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            logger = Verbose ? (IEasyGelfLogger)new VerboseLogger() : new SilentLogger();
            var mainTransport = InitializeTransport(logger);
            transport = new BufferedTransport(logger, UseRetry ? new RetryingTransport(logger, mainTransport, RetryCount, RetryDelay) : mainTransport);
        }

        protected override void CloseTarget()
        {
            base.CloseTarget();
            if (transport == null)
                return;
            transport.Close();
            transport = null;
        }


        private static GelfLevel ToGelf(LogLevel level)
        {
            if (level == LogLevel.Debug)
                return GelfLevel.Debug;
            if (level == LogLevel.Fatal)
                return GelfLevel.Critical;
            if (level == LogLevel.Info)
                return GelfLevel.Informational;
            if (level == LogLevel.Trace)
                return GelfLevel.Informational;
            return level == LogLevel.Warn ? GelfLevel.Warning : GelfLevel.Error;
        }
    }
}
