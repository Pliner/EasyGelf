namespace EasyGelf.Core
{
    public interface ITransport
    {
        void Send(byte[] bytes);
        void Close();
    }
}