using System.Threading.Tasks;

namespace EasyGelf.Core.Transports
{
    public interface ITransport
    {
        Task Send(GelfMessage message);
        void Close();
    }
}