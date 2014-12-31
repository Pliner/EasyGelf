using JetBrains.Annotations;

namespace EasyGelf.Core
{
    public interface ITransport
    {
        void Send([NotNull]GelfMessage message);
        void Close();
    }
}