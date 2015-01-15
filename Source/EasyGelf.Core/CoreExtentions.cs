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

        [ThreadStatic]
        private static Random random;

        private static Random Random { get { return random ?? (random = new Random()); } }

        public static T[] Shuffle<T>(this T[] array)
        {
            for (var i = array.Length - 1; i > 0; --i)
            {
                var k = Random.Next(i + 1);
                var e = array[i];
                array[i] = array[k];
                array[k] = e;
            }
            return array;
        }


        private const int MaxMessageSize = 8192;
        private const int MinMessageSize = 512;

        public static int UdpMessageSize(this int messageSize)
        {
            return Math.Max(MinMessageSize, Math.Min(MaxMessageSize, messageSize));
        }
    }

}