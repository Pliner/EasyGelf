using System;
using System.Collections.Generic;

namespace EasyGelf.Core
{
    public class GelfMessageBuilder : IGelfMessageBuilder
    {
        private readonly Dictionary<string, string> additionalFields = new Dictionary<string, string>();
        private readonly string message;
        private readonly string host;
        private DateTime? timestamp;
        private GelfLevel? level;

        public GelfMessageBuilder(string message, string host)
        {
            this.message = message;
            this.host = host;
        }

        public IGelfMessageBuilder SetAdditionalField(string key, string value)
        {
            additionalFields.Add(key, value);
            return this;
        }

        public IGelfMessageBuilder SetTimestamp(DateTime value)
        {
            timestamp = value;
            return this;
        }

        public IGelfMessageBuilder SetLevel(GelfLevel value)
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