using System.IO;
using System.Net;

namespace EasyGelf.Core.Http
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
            var request = WebRequest.CreateHttp(configuration.Uri);
            using (var requestStream = request.GetRequestStream())
            using (var messageStream = new MemoryStream(messageSerializer.Serialize(message)))
                messageStream.CopyTo(requestStream);
            request.Method = "POST";
            request.AllowAutoRedirect = false;
            request.ReadWriteTimeout = request.Timeout = request.ContinueTimeout = configuration.Timeout;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.Accepted)
                    return;
                throw new SendFailedException();
            }
        }

        public void Close()
        {
        }
    }
}