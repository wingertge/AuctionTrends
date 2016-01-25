using System.IO;

namespace AuctionTrends.Common.Models.New
{
    public class DataPoint
    {
        public string RealmName { get; set; }
        public string RealmSlug { get; set; }
        public double TimeStamp { get; set; }

        public void Serialize(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(RealmName);
            writer.Write(RealmSlug);
            writer.Write(TimeStamp);
        }

        public static DataPoint Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var dataPoint = new DataPoint();
            dataPoint.RealmName = reader.ReadString();
            dataPoint.RealmSlug = reader.ReadString();
            dataPoint.TimeStamp = reader.ReadDouble();

            return dataPoint;
        }
    }
}