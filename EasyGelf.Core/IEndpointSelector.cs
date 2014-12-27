using System.Net;

namespace EasyGelf.Core
{
    public interface IEndpointSelector
    {
        IPEndPoint GetEnpoint(IPEndPoint[] topology);
    }
}