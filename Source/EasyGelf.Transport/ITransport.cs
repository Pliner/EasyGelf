using EasyGelf.Core;

namespace EasyGelf.Transport
{
    public interface ITransport
    {
        void Send(GelfMessage message);
        void Close();
    }
}