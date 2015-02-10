using System;
using System.Text;

namespace EasyGelf.Core
{
    public sealed class GelfMessageSerializer : IGelfMessageSerializer
    {
        public byte[] Serialize(GelfMessage message)
        {
            var duration = message.Timestamp.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            var result = new JsonObject
                {
                    {"version", message.Version},
                    {"host", message.Host},
                    {"short_message", message.ShortMessage},
                    {"full_message", message.FullMessage},
                    {"timestamp", (long)duration.TotalSeconds},
                    {"level", (int)message.Level}
                };
            foreach (var additionalField in message.AdditionalFields)
            {
                var key = additionalField.Key;
                var value = additionalField.Value;
                result.Add(key.StartsWith("_") ? key : "_" + key, value);
            }
            return Encoding.UTF8.GetBytes(result.ToString());
        }
    }
}