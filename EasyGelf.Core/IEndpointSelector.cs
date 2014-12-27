using System.Net;
using JetBrains.Annotations;

namespace EasyGelf.Core
{
    public interface IEndpointSelector
    {
        [NotNull]
        IPEndPoint GetEnpoint([NotNull]IPEndPoint[] topology);
    }
}