namespace EasyGelf.Core.Transports
{
    public interface ITransport
    {
        void Send(GelfMessage message);
        void Close();
    }
}