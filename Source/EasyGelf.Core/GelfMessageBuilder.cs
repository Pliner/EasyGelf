using System;
using System.Collections.Generic;

namespace EasyGelf.Core
{
    public sealed class GelfMessageBuilder
    {
        private readonly Dictionary<string, string> additionalFields = new Dictionary<string, string>();
        private readonly string message;
        private readonly string host;
        private DateTime? timestamp;
        private GelfLevel? level;

        public GelfMessageBuilder(string message, string host)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("message");
            this.message = message;
            this.host = host;
        }

        public GelfMessageBuilder SetAdditionalField(string key, string value)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");
            additionalFields.Add(key, value);
            return this;
        }

        public GelfMessageBuilder SetTimestamp(DateTime value)
        {
            timestamp = value;
            return this;
        }

        public GelfMessageBuilder SetLevel(GelfLevel value)
        {
            level = value;
            return this;
        }

        public GelfMessage ToMessage()
        {
            if(timestamp == null)
                throw new ArgumentNullException();
            if(level == null)
                throw new ArgumentNullException();

            return new GelfMessage
                {
                    Host = host,
                    FullMessage = message,
                    ShortMessage = message.Truncate(200),
                    Level = level.Value,
                    Timestamp = timestamp.Value,
                    AdditionalFields = additionalFields
                };
        }
    }
}