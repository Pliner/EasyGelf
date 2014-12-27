using System.Net;

namespace EasyGelf.Core
{
    public class AbstractTransportConfiguration : IAbstractTransportConfiguration
    {
        public AbstractTransportConfiguration()
        {
            LargeMessageSize = 1024;
            MessageChunkSize = 1024;
            SplitLargeMessages = true;
        }

        public int LargeMessageSize { get; set; }
        public int MessageChunkSize { get; set; }
        public bool SplitLargeMessages { get; set; }
        public IPEndPoint[] Topology { get; set; }
    }
}