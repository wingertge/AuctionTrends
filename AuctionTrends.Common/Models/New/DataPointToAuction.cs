using System;
using System.IO;
using AuctionTrends.Common.Util;

namespace AuctionTrends.Common.Models.New
{
    public class DataPointToAuction
    {
        public Guid Id { get; set; }
        public double DataPointTimeStamp { get; set; }
        public string DataPointRealmSlug { get; set; }
        public long Auction { get; set; }
        public string TimeLeft { get; set; }

        public void Serialize(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(Id);
            writer.Write(DataPointTimeStamp);
            writer.Write(DataPointRealmSlug);
            writer.Write(Auction);
            writer.Write(TimeLeft);
        }

        public static DataPointToAuction Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var snapshot = new DataPointToAuction();
            snapshot.Id = reader.ReadGuid();
            snapshot.DataPointTimeStamp = reader.ReadDouble();
            snapshot.DataPointRealmSlug = reader.ReadString();
            snapshot.Auction = reader.ReadInt64();
            snapshot.TimeLeft = reader.ReadString();

            return snapshot;
        }
    }
}