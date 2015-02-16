using EasyGelf.Core;

namespace EasyGelf.Log4Net.Transport
{
    public interface ITransport
    {
        void Send(GelfMessage message);
        void Close();
    }
}