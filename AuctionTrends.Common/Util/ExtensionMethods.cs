using System;
using System.Collections.Generic;
using System.IO;

namespace AuctionTrends.Common.Util
{
    public static class ExtensionMethods
    {
        public static Guid ReadGuid(this BinaryReader reader)
        {
            var long1 = reader.ReadInt64();
            var long2 = reader.ReadInt64();
            var bytes = new byte[16];
            Array.Copy(BitConverter.GetBytes(long1), 0, bytes, 0, 8);
            Array.Copy(BitConverter.GetBytes(long2), 0, bytes, 8, 8);
            return new Guid(bytes);
        }

        public static void Write(this BinaryWriter writer, Guid guid)
        {
            var bytes = guid.ToByteArray();
            writer.Write(BitConverter.ToInt64(bytes, 0));
            writer.Write(BitConverter.ToInt64(bytes, 8));
        }
    }
}