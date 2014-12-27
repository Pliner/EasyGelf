using JetBrains.Annotations;

namespace EasyGelf.Core
{
    public interface IGelfMessageSerializer
    {
        [NotNull]
        string Serialize([NotNull]GelfMessage message);
    }
}