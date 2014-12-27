namespace EasyGelf.Core
{
    public interface IGelfMessageSerializer
    {
        string Serialize(GelfMessage message);
    }
}