namespace EasyGelf.Core
{
    public interface IIdGenerator
    {
        byte[] Generate(byte[] message);
    }
}