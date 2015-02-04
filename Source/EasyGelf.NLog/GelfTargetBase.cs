using System;
using System.Globalization;
using EasyGelf.Core;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace EasyGelf.NLog
{
    public abstract class GelfTargetBase : TargetWithLayout
    {
        private ITransport transport;

        public string Facility { get; set; }

        public string HostName { get; set; }

        public bool IncludeSource { get; set; }
       
        public bool UseRetry { get; set; }

        public int RetryCount { get; set; }

        public TimeSpan RetryDelay { get; set; }

        public bool IncludeStackTrace { get; set; }

        protected GelfTargetBase()
        {
            Facility = "gelf";
            HostName = Environment.MachineName;
            IncludeSource = true;
            UseRetry = true;
            RetryCount = 5;
            RetryDelay = TimeSpan.FromMilliseconds(50);
            IncludeStackTrace = true;
        }

        protected abstract ITransport InitializeTransport();

        protected override void Write(LogEventInfo loggingEvent)
        {
            try
            {
                var renderedEvent = Layout.Render(loggingEvent);
                var messageBuilder = new GelfMessageBuilder(renderedEvent, HostName)
                    .SetLevel(ToGelf(loggingEvent.Level))
                    .SetTimestamp(loggingEvent.TimeStamp)
                    .SetAdditionalField(GelfAdditionalFields.Facility, Facility)
                    .SetAdditionalField(GelfAdditionalFields.LoggerName, loggingEvent.LoggerName);
                if (IncludeSource)
                {
                    var userStackFrame = loggingEvent.UserStackFrame;
                    if (userStackFrame != null)
                    {
                        var fileName = userStackFrame.GetFileName();
                        if (!string.IsNullOrEmpty(fileName))
                            messageBuilder.SetAdditionalField(GelfAdditionalFields.SourceFileName, fileName);
                        messageBuilder.SetAdditionalField(GelfAdditionalFields.SourceLineNumber, userStackFrame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture));
                    }
                }
                if (IncludeStackTrace)
                {
                    var exception = loggingEvent.Exception;
                    if (exception != null)
                    {
                        var exceptionMessage = exception.Message;
                        if(!string.IsNullOrEmpty(exceptionMessage))
                            messageBuilder.SetAdditionalField(GelfAdditionalFields.ExceptionMessage, exceptionMessage);
                        var exceptionStackTrace = exception.StackTrace;
                        if (!string.IsNullOrEmpty(exceptionStackTrace))
                            messageBuilder.SetAdditionalField(GelfAdditionalFields.ExceptionStackTrace, exceptionMessage);
                    }
                }
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
            var mainTransport = InitializeTransport();
            transport = new BufferedTransport(UseRetry ? new RetryingTransport(mainTransport, RetryCount, RetryDelay) : mainTransport);
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
            if (level == LogLevel.Warn)
                return GelfLevel.Warning;
            return GelfLevel.Error;
        }
    }
}
