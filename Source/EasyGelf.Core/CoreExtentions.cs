using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyGelf.Core
{
    public static class CoreExtentions
    {
        public static void SafeDo(Action action)
        {
            try
            {
                action();
            }
            catch
            {
            }
        }

        public static string Truncate(this string message, int length)
        {
            return (message.Length > length)
                       ? message.Substring(0, length - 1)
                       : message;
        }

        [NotNull]
        public static string Serialize([NotNull]this GelfMessage message)
        {
            var duration = message.Timestamp.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            var result = new JObject
                {
                    {"version", message.Version},
                    {"host", message.Host},
                    {"short_message", message.ShortMessage},
                    {"full_message", message.FullMessage},
                    {"timestamp", duration.TotalSeconds},
                    {"level", (int)message.Level}
                };
            foreach (var additionalField in message.AdditionalFields)
            {
                var key = additionalField.Key;
                var value = additionalField.Value;
                result.Add(key.StartsWith("_") ? key : "_" + key, value);
            }
            return result.ToString(Formatting.None);
        }

        [NotNull]
        public static byte[] GenerateGelfId([NotNull]this byte[] message)
        {

            //create a bit array to store the entire message id (which is 8 bytes)
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