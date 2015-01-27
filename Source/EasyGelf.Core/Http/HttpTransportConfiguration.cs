using JetBrains.Annotations;

namespace EasyGelf.Core.Http
{
    public sealed class HttpTransportConfiguration
    {
        [NotNull]
        public string Uri { get; set; }
        public int Timeout { get; set; }
    }
}