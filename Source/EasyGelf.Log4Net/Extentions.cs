using EasyGelf.Core;
using log4net.Core;

namespace EasyGelf.Log4Net
{
    public static class Extentions
    {
        public static GelfLevel ToGelf(this Level level)
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
    }
}