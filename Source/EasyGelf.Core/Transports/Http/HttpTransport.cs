using System.IO;
using System.Net;

namespace EasyGelf.Core.Transports.Http
{
    public sealed class HttpTransport : ITransport
    {
        private readonly HttpTransportConfiguration configuration;
        private readonly IGelfMessageSerializer messageSerializer;

        public HttpTransport(HttpTransportConfiguration configuration, IGelfMessageSerializer messageSerializer)
        {
            this.configuration = configuration;
            this.messageSerializer = messageSerializer;
        }

        public void Send(GelfMessage message)
        {
            var request = (HttpWebRequest)WebRequest.Create(configuration.GetComposedUri());            
            request.Method = "POST";
            request.AllowAutoRedirect = false;
            request.ReadWriteTimeout = request.Timeout = configuration.Timeout;
            using (var requestStream = request.GetRequestStream())
            {
                using (var messageStream = new MemoryStream(messageSerializer.Serialize(message)))
                {
                    messageStream.CopyTo(requestStream);
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.Accepted)
                        return;
                    throw new SendFailedException();
                }
            }
        }

        public void Close()
        {
        }
    }
}