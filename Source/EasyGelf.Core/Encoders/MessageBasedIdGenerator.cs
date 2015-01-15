using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace EasyGelf.Core.Encoders
{
    //TODO Simplify message id generator
    public sealed class MessageBasedIdGenerator : IChunkedMessageIdGenerator
    {
        public byte[] GenerateId(byte[] message)
        {     //create a bit array to store the entire message id (which is 8 bytes)
            var bitArray = new BitArray(64);

            //Read the server ip address
            var ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            var ipAddress =
                (from ip in ipAddresses where ip.AddressFamily == AddressFamily.InterNetwork select ip).FirstOrDefault();

            if (ipAddress == null)
                throw new Exception("Cannot get an ipaddress");

            //read bytes of the last 2 segments and insert bits into the bit array
            var addressBytes = ipAddress.GetAddressBytes();
            AddToBitArray(bitArray, 0, addressBytes[2], 0, 8);
            AddToBitArray(bitArray, 8, addressBytes[3], 0, 8);

            //read the current second and insert 6 bits into the bit array
            var second = DateTime.Now.Second;
            AddToBitArray(bitArray, 16, (byte)second, 0, 6);

            //generate the MD5 hash of the compressed message
            byte[] hashOfCompressedMessage;
            using (var md5 = MD5.Create())
            {
                hashOfCompressedMessage = md5.ComputeHash(message);
            }

            //insert the first 42 bits into the bit array
            var startIndex = 22;
            for (var hashByteIndex = 0; hashByteIndex < 5; hashByteIndex++)
            {
                var hashByte = hashOfCompressedMessage[hashByteIndex];
                AddToBitArray(bitArray, startIndex, hashByte, 0, 8);
                startIndex += 8;
            }

            //copy all bits from bit array into a byte[]
            var result = new byte[8];
            bitArray.CopyTo(result, 0);

            return result;
        }

        private static void AddToBitArray(BitArray bitArray, int bitArrayIndex, byte byteData, int byteDataIndex, int length)
        {
            var localBitArray = new BitArray(new[] { byteData });

            for (var i = byteDataIndex + length - 1; i >= byteDataIndex; i--)
            {
                bitArray.Set(bitArrayIndex, localBitArray.Get(i));
                bitArrayIndex++;
            }
        }
    }
}