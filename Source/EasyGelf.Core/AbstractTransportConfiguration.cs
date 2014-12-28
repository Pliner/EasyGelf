namespace EasyGelf.Core
{
    public abstract class AbstractTransportConfiguration : IAbstractTransportConfiguration
    {
        protected AbstractTransportConfiguration()
        {
            LargeMessageSize = 1024;
            MessageChunkSize = 1024;
            SplitLargeMessages = true;
        }

        public int LargeMessageSize { get; set; }
        public int MessageChunkSize { get; set; }
        public bool SplitLargeMessages { get; set; }
    }
}