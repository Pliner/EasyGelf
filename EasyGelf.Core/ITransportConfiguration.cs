using System.Net;

namespace EasyGelf.Core
{
    public interface ITransportConfiguration
    {
        int LargeMessageSize { get; }    
        int MessageChunkSize { get; }
        bool SplitLargeMessages { get; }
        IPEndPoint[] Topology { get; }
    }
}