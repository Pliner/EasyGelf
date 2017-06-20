using System;
using System.Collections.Generic;
using System.IO;

namespace EasyGelf.Core.Encoders
{
    using System.Threading.Tasks;

    public sealed class ChunkingEncoder : ITransportEncoder
    {
        private const int HeaderSize = 12;
        private const int MaxChunkCount = 128;

        private readonly IChunkedMessageIdGenerator idGenerator;

        private readonly int maxSize;

        public ChunkingEncoder(IChunkedMessageIdGenerator idGenerator, int maxSize)
        {
            this.idGenerator = idGenerator;
            this.maxSize = maxSize;
        }

        public async Task<IEnumerable<byte[]>> Encode(byte[] bytes)
        {
            if (bytes.Length <= maxSize)
                return new[] { bytes };
            else
            {
                var messageChunkSize = maxSize - HeaderSize;
                var chunksCount = bytes.Length / messageChunkSize + 1;
                if(chunksCount > MaxChunkCount)
                    throw new ArgumentOutOfRangeException("bytes");
                var remainingBytes = bytes.Length;
                var messageId = await idGenerator.GenerateId(bytes);
                var result = new List<byte[]>();
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
                        result.Add(stream.ToArray());
                    }
                    remainingBytes -= chunkBytes;
                }

                return result;
            }
        }
    }
}