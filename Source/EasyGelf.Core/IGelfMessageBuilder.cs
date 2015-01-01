using System;

namespace EasyGelf.Core
{
    public interface IGelfMessageBuilder
    {
        IGelfMessageBuilder SetAdditionalField(string key, string value);
        IGelfMessageBuilder SetTimestamp(DateTime timestamp);
        IGelfMessageBuilder SetLevel(GelfLevel value);
        GelfMessage ToMessage();
    }
}