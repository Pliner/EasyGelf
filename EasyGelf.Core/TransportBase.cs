using System;
using System.IO;
using System.Net;

namespace EasyGelf.Core
{
    public abstract class TransportBase : ITransport
    {
        private const int HeaderSize = 12;

        private readonly ITransportConfiguration configuration;
        private readonly IEndpointSelector endpointSelector;
        private readonly IIdGenerator idGenerator;
        
        protected TransportBase(
            ITransportConfiguration configuration, 
            IEndpointSelector endpointSelector,
            IIdGenerator idGenerator)
        {
            this.configuration = configuration;
            this.endpointSelector = endpointSelector;
            this.idGenerator = idGenerator;
        }

        protected TransportBase(ITransportConfiguration configuration): this(configuration, new RandomEndpointSelector(), new IdGenerator())
        {
        }

        public void Send(byte[] bytes)
        {
            if(! SplitLargeMessage)
                SendInternal(bytes);
            else if (bytes.Length <= configuration.LargeMessageSize)
                SendInternal(bytes);
            else
            {
                var messageChunkSize = configuration.MessageChunkSize - HeaderSize;
                var chunksCount = bytes.Length / messageChunkSize + 1;
                var remainingBytes = bytes.Length;
                var messageId = idGenerator.Generate(bytes);
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

        protected virtual bool SplitLargeMessage
        {
            get { return configuration.SplitLargeMessages; }
        }

        private void SendInternal(byte[] bytes)
        {
            var endPoint = endpointSelector.GetEnpoint(configuration.Topology);
            SendToEndpoint(endPoint, bytes);
        }

        public abstract void SendToEndpoint(IPEndPoint endPoint, byte[] bytes);

        public abstract void Close();
    }
}