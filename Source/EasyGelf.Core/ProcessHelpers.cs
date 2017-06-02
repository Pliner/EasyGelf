using System.Diagnostics;

namespace EasyGelf.Core
{
    public static class ProcessHelpers
    {
        public static string ProcessName
        {
            get
            {
                var process = Process.GetCurrentProcess();
                return process.ProcessName;
            }
        }
    }
}