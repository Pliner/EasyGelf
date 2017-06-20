
namespace EasyGelf.Core.Transports.Http
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class HttpTransport : ITransport
    {
        private readonly HttpTransportConfiguration configuration;
        private readonly IGelfMessageSerializer messageSerializer;

        private HttpClient httpClient;

        public HttpTransport(HttpTransportConfiguration configuration, IGelfMessageSerializer messageSerializer)
        {
            this.configuration = configuration;
            this.messageSerializer = messageSerializer;
        }

        public async Task Send(GelfMessage message)
        {
            this.PrepareHttpClient();

            var content = new ByteArrayContent(messageSerializer.Serialize(message));
            using (var response = await this.httpClient.PostAsync(this.configuration.Uri, content))
            {
                if (response.StatusCode == HttpStatusCode.Accepted)
                    return;
                throw new SendFailedException();
            }
        }

        public void Close()
        {
            this.httpClient.SafeDispose();
        }

        private void PrepareHttpClient()
        {
            if (this.httpClient != null) return;

            var httpHandler = new HttpClientHandler { AllowAutoRedirect = false };
            this.httpClient = new HttpClient(httpHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(this.configuration.Timeout)
            };
        }
    }
}