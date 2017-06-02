using System;
using EasyGelf.Core;
using EasyGelf.Core.Transports;
using log4net.Appender;
using log4net.Core;

namespace EasyGelf.Log4Net
{
    public abstract class GelfAppenderBase : AppenderSkeleton
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

        public bool IncludeEventProperties { get; set; }

        protected GelfAppenderBase()
        {
            Facility = ProcessHelpers.ProcessName;
            IncludeSource = true;
            IncludeEventProperties = true;
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
                logger = Verbose ? (IEasyGelfLogger) new VerboseLogger() : new SilentLogger();
                var mainTransport = InitializeTransport(logger);
                var selectedTransport = UseRetry
                    ? new RetryingTransport(logger, mainTransport, RetryCount, RetryDelay)
                    : mainTransport;

                transport = new BufferedTransport(logger, selectedTransport);
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
                var level = loggingEvent.Level.ToGelf();
                var renderedEvent = RenderLoggingEvent(loggingEvent);
                var messageBuilder = new GelfMessageBuilder(renderedEvent, HostName, loggingEvent.TimeStamp, level)
                    .SetAdditionalField("facility", Facility)
                    .SetAdditionalField("loggerName", loggingEvent.LoggerName)
                    .SetAdditionalField("threadName", loggingEvent.ThreadName)
                    .SetAdditionalField("userName", loggingEvent.UserName)
                    .SetAdditionalField("appDomain", loggingEvent.Domain);
                
                if (IncludeSource)
                {
                    var locationInformation = loggingEvent.LocationInformation;
                    if (locationInformation != null)
                    {
                        messageBuilder.SetAdditionalField("sourceFileName", locationInformation.FileName)
                            .SetAdditionalField("sourceClassName", locationInformation.ClassName)
                            .SetAdditionalField("sourceMethodName", locationInformation.MethodName)
                            .SetAdditionalField("sourceLineNumber", locationInformation.LineNumber);
                    }
                }
                
                if (IncludeStackTrace)
                {
                    var exception = loggingEvent.ExceptionObject;
                    if (exception != null)
                    {
                        messageBuilder.SetAdditionalField("exceptionType", exception.GetType().FullName);
                        messageBuilder.SetAdditionalField("exceptionMessage", exception.Message);
                        messageBuilder.SetAdditionalField("exceptionStackTrace", exception.StackTrace);
                    }
                }

                if (IncludeEventProperties)
                {
                    var properties = loggingEvent.Properties;
                    foreach (var propertyKey in properties.GetKeys())
                    {
                        messageBuilder.SetAdditionalField(propertyKey, properties[propertyKey].ToString());
                    }
                }

                transport.Send(messageBuilder.ToMessage());
            }
            catch (Exception exception)
            {
                logger.Error("Unable to send logging event to remote host", exception);
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