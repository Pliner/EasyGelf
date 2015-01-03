using JetBrains.Annotations;

namespace EasyGelf.Core
{
    public interface IGelfMessageSerializer
    {
        [NotNull]
        byte[] Serialize([NotNull] GelfMessage message);
    }
}