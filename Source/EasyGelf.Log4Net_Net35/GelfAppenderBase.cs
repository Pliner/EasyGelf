using System;
using EasyGelf.Core;
using EasyGelf.Log4Net.Transport;
using log4net.Appender;
using log4net.Core;

namespace EasyGelf.Log4Net
{
    public abstract class GelfAppenderBase  : AppenderSkeleton
    {
        private ITransport transport;
        private IEasyGelfLogger logger;

        public string Facility { get; set; }

        public bool IncludeSource { get; set; }

        public string HostName { get; set; }

        public bool UseRetry { get; set; }

        public int RetryCount { get; set; }

        public TimeSpan RetryDelay { get; set; }

        public bool IncludeStackTrace { get; set; }

        public bool Verbose { get; set; }

        protected GelfAppenderBase()
        {
            Facility = "gelf";
            IncludeSource = true;
            Verbose = false;
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
                logger = Verbose ? (IEasyGelfLogger)new VerboseLogger() : new SilentLogger();
                var mainTransport = InitializeTransport(logger);
                transport = mainTransport;
            }
            catch (Exception exception)
            {
                logger.Error("Failed to create Transport", exception);
            }
        }

        protected override bool RequiresLayout
        {
            get { return true; }
        }

        protected abstract ITransport InitializeTransport(IEasyGelfLogger logger);

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                var renderedEvent = RenderLoggingEvent(loggingEvent);
                var messageBuilder = new GelfMessageBuilder(renderedEvent, HostName, loggingEvent.TimeStamp, ToGelf(loggingEvent.Level))
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
                        messageBuilder.SetAdditionalField(GelfAdditionalFields.ExceptionMessage, exception.Message);
                        messageBuilder.SetAdditionalField(GelfAdditionalFields.ExceptionStackTrace, exception.StackTrace);
                    }
                }
                transport.Send(messageBuilder.ToMessage());
            }
            catch (Exception exception)
            {
                logger.Error("Unable to send logging event to remote host", exception);
            }
        }

        public static GelfLevel ToGelf( Level level)
        {
            if (level == Level.Alert)
                return GelfLevel.Alert;

            if (level == Level.Critical || level == Level.Fatal)
                return GelfLevel.Critical;

            if (level == Level.Debug)
                return GelfLevel.Debug;

            if (level == Level.Emergency)
                return GelfLevel.Emergency;

            if (level == Level.Error)
                return GelfLevel.Error;

            if (level == Level.Fine
                || level == Level.Finer
                || level == Level.Finest
                || level == Level.Info
                || level == Level.Off)
                return GelfLevel.Informational;

            if (level == Level.Notice
                || level == Level.Verbose
                || level == Level.Trace)
                return GelfLevel.Notice;

            if (level == Level.Severe)
                return GelfLevel.Emergency;

            if (level == Level.Warn)
                return GelfLevel.Warning;

            return GelfLevel.Debug;
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