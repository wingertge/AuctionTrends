using System;
using System.IO;
using AuctionTrends.Common.Util;

namespace AuctionTrends.Common.Models.New
{
    public class Auction
    {
        private const int EndBytes = 1324869345;

        public long Id { get; set; }
        public long ItemId { get; set; }
        public string PlacedBy { get; set; }
        public string Realm { get; set; }
        public long CurrentBid { get; set; }
        public long Buyout { get; set; }
        public int Quantity { get; set; }
        public long Random { get; set; }
        public long Seed { get; set; }
        public int GenerationContext { get; set; }
        public int? PetSpeciesId { get; set; }
        public int? PetBreedId { get; set; }
        public int? PetLevel { get; set; }
        public int? PetQualityId { get; set; }

        public void Serialize(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(Id);
            writer.Write(ItemId);
            writer.Write(PlacedBy);
            writer.Write(Realm);
            writer.Write(CurrentBid);
            writer.Write(Buyout);
            writer.Write(Quantity);
            writer.Write(Random);
            writer.Write(Seed);
            writer.Write(GenerationContext);
            writer.Write(PetSpeciesId ?? -1);
            writer.Write(PetBreedId ?? -1);
            writer.Write(PetLevel ?? -1);
            writer.Write(PetQualityId ?? -1);
        }

        public static Auction Deserialize(Stream dataStream)
        {
            var reader = new BinaryReader(dataStream);
            var auction = new Auction();
            auction.Id = reader.ReadInt64();
            auction.ItemId = reader.ReadInt64();
            auction.PlacedBy = reader.ReadString();
            auction.Realm = reader.ReadString();
            auction.CurrentBid = reader.ReadInt64();
            auction.Buyout = reader.ReadInt64();
            auction.Quantity = reader.ReadInt32();
            auction.Random = reader.ReadInt64();
            auction.Seed = reader.ReadInt64();
            auction.GenerationContext = reader.ReadInt32();
            auction.PetSpeciesId = reader.ReadInt32();
            auction.PetBreedId = reader.ReadInt32();
            auction.PetLevel = reader.ReadInt32();
            auction.PetQualityId = reader.ReadInt32();

            if (auction.PetSpeciesId == -1) auction.PetSpeciesId = null;
            if (auction.PetBreedId == -1) auction.PetBreedId = null;
            if (auction.PetLevel == -1) auction.PetLevel = null;
            if (auction.PetQualityId == -1) auction.PetQualityId = null;

            return auction;
        }
    }

    public class Upgrade
    {
        public Guid Id { get; set; }
        public int UpgradeId { get; set; }
        public long Auction { get; set; }

        public void Serialize(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(Id);
            writer.Write(UpgradeId);
            writer.Write(Auction);
        }

        public static Upgrade Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var upgrade = new Upgrade();
            upgrade.Id = reader.ReadGuid();
            upgrade.UpgradeId = reader.ReadInt32();
            upgrade.Auction = reader.ReadInt64();

            return upgrade;
        }
    }

    public class Modifier
    {
        public Guid Id { get; set; }
        public int TypeId { get; set; }
        public int Value { get; set; }
        public long Auction { get; set; }

        public void Serialize(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(Id);
            writer.Write(TypeId);
            writer.Write(Value);
            writer.Write(Auction);
        }

        public static Modifier Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var modifier = new Modifier();
            modifier.Id = reader.ReadGuid();
            modifier.TypeId = reader.ReadInt32();
            modifier.Value = reader.ReadInt32();
            modifier.Auction = reader.ReadInt64();

            return modifier;
        }
    }
}