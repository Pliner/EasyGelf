using System;
using EasyGelf.Core;
using EasyGelf.Core.Transports;
using log4net.Appender;
using log4net.Core;
using System.Collections;
using System.Collections.Generic;

namespace EasyGelf.Log4Net
{
    public abstract class GelfAppenderBase : AppenderSkeleton
    {

        /// <summary>
        /// The class contains properties for output as static fields.
        /// </summary>
        public class StaticOutField
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public void AddStaticOutField(StaticOutField additionalParameter)
        {
            _staticOutFields.Add(additionalParameter);
        }

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

        protected List<StaticOutField> _staticOutFields;

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
            _staticOutFields = new List<StaticOutField>();
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            try
            {
                logger = Verbose ? (IEasyGelfLogger)new VerboseLogger() : new SilentLogger();
                var mainTransport = InitializeTransport(logger);
                transport = new BufferedTransport(logger, UseRetry ? new RetryingTransport(logger, mainTransport, RetryCount, RetryDelay) : mainTransport);
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
                var messageBuilder = new GelfMessageBuilder(renderedEvent, HostName, loggingEvent.TimeStamp, loggingEvent.Level.ToGelf())
                    .SetAdditionalField(GelfAdditionalFields.Facility, Facility)
                    .SetAdditionalField(GelfAdditionalFields.LoggerName, loggingEvent.LoggerName)
                    .SetAdditionalField(GelfAdditionalFields.ThreadName, loggingEvent.ThreadName);

                foreach (var item in _staticOutFields)
                {
                    messageBuilder.SetAdditionalField(item.Name, item.Value);
                }

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