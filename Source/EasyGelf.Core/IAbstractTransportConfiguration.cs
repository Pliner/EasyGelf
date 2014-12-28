namespace EasyGelf.Core
{
    public interface IAbstractTransportConfiguration
    {
        int LargeMessageSize { get; }    
        int MessageChunkSize { get; }
        bool SplitLargeMessages { get; }
    }
}