using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using AuctionTrends.Common.Models.New;

namespace AuctionTrends.Server
{
    public static class Util
    {
        private const string AuctionIdsPath = "/auction_ids.dat";
        private const string DataPointsPath = "/data_points.dat";
        private const string SnapshotsPath = "/snapshots.dat";
        private const string AuctionsPath = "/auctions.dat";
        private const string UpgradesPath = "/upgrades.dat";
        private const string ModifiersPath = "/modifiers.dat";

        public static void SaveData(string realmSlug, DataPoint dataPoint, List<DataPointToAuction> snapshots, List<Auction> auctions, List<Upgrade> upgrades, List<Modifier> modifiers)
        {
            var dir = $"data/{realmSlug}";
            var auctionIds = GetAuctionIds(realmSlug);

            if (!Directory.Exists("data")) Directory.CreateDirectory("data");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            using (var stream = new FileStream(dir + DataPointsPath, FileMode.Append))
                dataPoint.Serialize(stream);

            using (var stream = new FileStream(dir + SnapshotsPath, FileMode.Append))
                foreach (var snapshot in snapshots)
                    snapshot.Serialize(stream);

            using (var stream = new FileStream(dir + AuctionsPath, FileMode.Append))
            {
                using (var idsStream = new FileStream(dir + AuctionIdsPath, FileMode.Append))
                {
                    foreach (var auction in auctions.Where(auction => !auctionIds.Contains(auction.Id)))
                    {
                        auction.Serialize(stream);
                        idsStream.Write(BitConverter.GetBytes(auction.Id), 0, 8);
                    }
                }
            }

            using (var stream = new FileStream(dir + UpgradesPath, FileMode.Append))
                foreach (var upgrade in upgrades.Where(upgrade => !auctionIds.Contains(upgrade.Auction)))
                    upgrade.Serialize(stream);

            using (var stream = new FileStream(dir + ModifiersPath, FileMode.Append))
                foreach (var modifier in modifiers.Where(a => !auctionIds.Contains(a.Auction)))
                    modifier.Serialize(stream);
        }

        private static List<long> GetAuctionIds(string realmSlug)
        {
            var dir = $"data/{realmSlug}";
            if (!File.Exists(dir + AuctionIdsPath)) return new List<long>();
            var idData = File.OpenRead(dir + AuctionIdsPath);
            var auctionIds = new List<long>();
            while (idData.Position != idData.Length)
            {
                var bytes = new byte[8];
                for (var i = 0; i < bytes.Length; i++)
                    bytes[i] = (byte) idData.ReadByte();
                var value = BitConverter.ToInt64(bytes, 0);
                if(value != 0) auctionIds.Add(value);
            }

            return auctionIds;
        }
    }
}