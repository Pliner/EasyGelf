using System;
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

        [UsedImplicitly]
        public bool IncludeSource { get; set; }

        [UsedImplicitly]
        public string HostName { get; set; }

        [UsedImplicitly]
        public bool UseRetry { get; set; }

        [UsedImplicitly]
        public int RetryCount { get; set; }

        [UsedImplicitly]
        public TimeSpan RetryDelay { get; set; }

        [UsedImplicitly]
        public bool IncludeStackTrace { get; set; }

        protected GelfAppenderBase()
        {
            Facility = "gelf";
            IncludeSource = true;
            HostName = Environment.MachineName;
            UseRetry = true;
            RetryCount = 5;
            RetryDelay = TimeSpan.FromMilliseconds(50);
            IncludeStackTrace = true;
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            try
            {
                var mainTransport = InitializeTransport();
                transport = new BufferedTransport(UseRetry ? new RetryingTransport(mainTransport, RetryCount, RetryDelay) : mainTransport);
            }
            catch (Exception exception)
            {
                ErrorHandler.Error("Failed to create Transport", exception);
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
                var messageBuilder = new GelfMessageBuilder(renderedEvent, HostName)
                    .SetLevel(loggingEvent.Level.ToGelf())
                    .SetTimestamp(loggingEvent.TimeStamp)
                    .SetAdditionalField(GelfAdditionalFields.Facility, Facility)
                    .SetAdditionalField(GelfAdditionalFields.LoggerName, loggingEvent.LoggerName)
                    .SetAdditionalField(GelfAdditionalFields.ThreadName, loggingEvent.ThreadName);
                if (IncludeSource)
                {
                    var locationInformation = loggingEvent.LocationInformation;
                    if (locationInformation != null)
                    {
                        messageBuilder.SetAdditionalField(GelfAdditionalFields.SourceFileName, locationInformation.FileName)
                            .SetAdditionalField(GelfAdditionalFields.SourceClassName, locationInformation.ClassName)
                            .SetAdditionalField(GelfAdditionalFields.SourceMethodName, locationInformation.MethodName)
                            .SetAdditionalField(GelfAdditionalFields.SourceLineNumber, locationInformation.LineNumber);
                    }
                }
                if (IncludeStackTrace)
                {
                    var exception = loggingEvent.ExceptionObject;
                    if (exception != null)
                    {
                        var exceptionMessage = exception.Message;
                        if (!string.IsNullOrEmpty(exceptionMessage))
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
                ErrorHandler.Error("Unable to send logging event to remote host", exception, ErrorCode.WriteFailure);
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
    }
}