using JetBrains.Annotations;

namespace EasyGelf.Core
{
    public interface ITransport
    {
        void Send([NotNull]byte[] bytes);
        void Close();
    }
}