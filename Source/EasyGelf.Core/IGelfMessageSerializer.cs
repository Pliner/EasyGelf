
namespace EasyGelf.Core
{
    public interface IGelfMessageSerializer
    {
        byte[] Serialize(GelfMessage message);
    }
}