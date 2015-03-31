
namespace EasyGelf.Core.Transports.Http
{
    public sealed class HttpTransportConfiguration
    {
        public string Uri { get; set; }
        public int Timeout { get; set; }
    }
}