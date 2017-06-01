namespace EasyGelf.Core.Transports.Tcp
{
    public class TcpTransportFactory
    {
        public static ITransport Produce(TcpTransportConfiguration configuration)
        {
            return configuration.Ssl 
                ? new TcpTransport(configuration, new GelfMessageSerializer(), () => new TcpSslConnection(configuration)) 
                : new TcpTransport(configuration, new GelfMessageSerializer(), () => new TcpConnection(configuration));
        }
    }
}
