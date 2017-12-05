
using System.Net;

namespace EasyGelf.Core.Transports.Http
{
    public sealed class HttpTransportConfiguration
    {
        public string Uri { get; set; }
        public int Timeout { get; set; }
        public int Port { get; set; }

        public string GetComposedUri()
        {
            return this.Uri + ":" + this.Port + "/gelf";
        }

    }
}