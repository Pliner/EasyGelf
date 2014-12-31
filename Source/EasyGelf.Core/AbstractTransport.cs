using System;
using System.IO;

namespace EasyGelf.Core
{
    public abstract class AbstractTransport : ITransport
    {
        private const int HeaderSize = 12;

        private readonly IAbstractTransportConfiguration configuration;
        
        protected AbstractTransport(IAbstractTransportConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Send(byte[] bytes)
        {
            if (bytes.Length <= configuration.MaxMessageSize)
                SendInternal(bytes);
            else
            {
                var messageChunkSize = configuration.MaxMessageSize - HeaderSize;
                var chunksCount = bytes.Length / messageChunkSize + 1;
                var remainingBytes = bytes.Length;
                var messageId = bytes.GenerateGelfId();
                for (var chunkSequenceNumber = 0; chunkSequenceNumber < chunksCount; ++chunkSequenceNumber)
                {
                    var chunkOffset = chunkSequenceNumber*messageChunkSize;
                    var chunkBytes = Math.Min(messageChunkSize, remainingBytes);
                    using (var stream = new MemoryStream(messageChunkSize))
                    {
                        stream.WriteByte(0x1e);
                        stream.WriteByte(0x0f);
                        stream.Write(messageId, 0, messageId.Length);
                        stream.WriteByte((byte)chunkSequenceNumber);
                        stream.WriteByte((byte)chunksCount);
                        stream.Write(bytes, chunkOffset, chunkBytes);
                        SendInternal(stream.ToArray());
                    }
                    remainingBytes -= chunkBytes;
                }
            }
        }

        protected abstract void SendInternal(byte[] bytes);

        public abstract void Close();
    }
}