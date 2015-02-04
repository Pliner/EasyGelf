namespace EasyGelf.Core.Encoders
{
    public interface IChunkedMessageIdGenerator
    {
        byte[] GenerateId(byte[] message);
    }
}