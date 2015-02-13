using System;
using System.Collections.Generic;

namespace EasyGelf.Core
{
    public sealed class GelfMessageBuilder
    {
        private readonly Dictionary<string, string> additionalFields = new Dictionary<string, string>();
        private readonly string message;
        private readonly string host;
        private readonly DateTime timestamp;
        private readonly GelfLevel level;

        public GelfMessageBuilder(string message, string host, DateTime timestamp, GelfLevel level)
        {
            this.message = message;
            this.host = host;
            this.timestamp = timestamp;
            this.level = level;
        }

        public GelfMessageBuilder SetAdditionalField(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return this;
            if (string.IsNullOrEmpty(value))
                return this;
            additionalFields.Add(key, value);
            return this;
        }

        public GelfMessage ToMessage()
        {
            return new GelfMessage
            {
                Host = host,
                FullMessage = message,
                ShortMessage = message.Truncate(200),
                Level = level,
                Timestamp = timestamp,
                AdditionalFields = additionalFields
            };
        }
    }
}