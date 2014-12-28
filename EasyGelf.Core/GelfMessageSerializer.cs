using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyGelf.Core
{
    public static class GelfMessageSerializer
    {
        [NotNull]
        public static string Serialize([NotNull]this GelfMessage message)
        {
            var duration = message.Timestamp.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            var result = new JObject
                {
                    {"version", message.Version},
                    {"host", message.Host},
                    {"short_message", message.ShortMessage},
                    {"full_message", message.FullMessage},
                    {"timestamp", duration.TotalSeconds},
                    {"level", (int)message.Level}
                };
            return result.ToString(Formatting.None);
        }
    }
}
