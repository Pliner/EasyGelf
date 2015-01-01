using System;
using System.Collections.Generic;

namespace EasyGelf.Core
{
    public sealed class GelfMessage
    {
        public GelfMessage()
        {
            AdditionalFields = new Dictionary<string, string>();
        }

        public string Version { get { return "1.1"; } }
        public string Host { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public GelfLevel Level { get; set; }
        public Dictionary<string, string> AdditionalFields { get; set; }
    }
}
