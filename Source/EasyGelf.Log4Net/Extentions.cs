using System;
using System.IO;
using System.IO.Compression;
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

        public static string Truncate(this string message, int length)
        {
            return (message.Length > length)
                       ? message.Substring(0, length - 1)
                       : message;
        }

        public static byte[] GZip(this byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var compressed = new GZipStream(output, CompressionMode.Compress))
                    input.CopyTo(compressed);
                return output.ToArray();
            }
        }
    }
}