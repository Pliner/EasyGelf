using System;
using System.Collections.Generic;
using System.IO;

namespace EasyGelf.Core.Encoders
{
    public sealed class ChunkingEncoder : ITransportEncoder
    {
        private const int HeaderSize = 12;
        private readonly int maxSize;

        public ChunkingEncoder(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public IEnumerable<byte[]> Encode(byte[] bytes)
        {
            if (bytes.Length <= maxSize)
                yield return bytes;
            else
            {
                var messageChunkSize = maxSize - HeaderSize;
                var chunksCount = bytes.Length / messageChunkSize + 1;
                var remainingBytes = bytes.Length;
                var messageId = bytes.GenerateGelfId();
                for (var chunkSequenceNumber = 0; chunkSequenceNumber < chunksCount; ++chunkSequenceNumber)
                {
                    var chunkOffset = chunkSequenceNumber * messageChunkSize;
                    var chunkBytes = Math.Min(messageChunkSize, remainingBytes);
                    using (var stream = new MemoryStream(messageChunkSize))
                    {
                        stream.WriteByte(0x1e);
                        stream.WriteByte(0x0f);
                        stream.Write(messageId, 0, messageId.Length);
                        stream.WriteByte((byte)chunkSequenceNumber);
                        stream.WriteByte((byte)chunksCount);
                        stream.Write(bytes, chunkOffset, chunkBytes);
                        yield return stream.ToArray();
                    }
                    remainingBytes -= chunkBytes;
                }
            }
        }
    }
}