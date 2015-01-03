using System;

namespace EasyGelf.Core
{
    public static class CoreExtentions
    {
        public static void SafeDo(Action action)
        {
            try
            {
                action();
            }
            catch
            {
            }
        }

        public static string Truncate(this string message, int length)
        {
            return (message.Length > length)
                       ? message.Substring(0, length - 1)
                       : message;
        }
    }

}