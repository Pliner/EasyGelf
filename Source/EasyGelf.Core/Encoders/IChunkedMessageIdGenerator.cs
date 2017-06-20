namespace EasyGelf.Core.Encoders
{
    using System.Threading.Tasks;

    public interface IChunkedMessageIdGenerator
    {
        Task<byte[]> GenerateId(byte[] message);
    }
}