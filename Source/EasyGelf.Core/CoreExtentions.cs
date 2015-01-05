using System;

namespace EasyGelf.Core
{
    public static class CoreExtentions
    {
        public static string Truncate(this string message, int length)
        {
            return (message.Length > length)
                       ? message.Substring(0, length - 1)
                       : message;
        }

        public static void SafeDispose(this IDisposable disposable)
        {
            if (disposable == null)
                return;
            try
            {
                disposable.Dispose();
            }
            catch
            {
            }
        }
    }

}