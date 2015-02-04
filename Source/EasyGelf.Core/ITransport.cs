namespace EasyGelf.Core
{
    public interface ITransport
    {
        void Send(GelfMessage message);
        void Close();
    }
}