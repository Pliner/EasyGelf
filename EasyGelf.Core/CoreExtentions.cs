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
    }
}