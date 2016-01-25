using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentNHibernate.Mapping;

namespace AuctionTrends.Common.Models
{
    [Serializable]
    public class Auction
    {
        public virtual long Id { get; set; }
        public virtual long ItemId { get; set; }
        public virtual string PlacedBy { get; set; }
        public virtual string Realm { get; set; }
        public virtual long CurrentBid { get; set; }
        public virtual long Buyout { get; set; }
        public virtual int Quantity { get; set; }
        public virtual long Random { get; set; }
        public virtual long Seed { get; set; }
        public virtual int GenerationContext { get; set; }
        public IList<Upgrade> UpgradeList { get; set; }
        public virtual IList<Modifier> Modifiers { get; set; }
        public virtual int? PetSpeciesId { get; set; }
        public virtual int? PetBreedId { get; set; }
        public virtual int? PetLevel { get; set; }
        public virtual int? PetQualityId { get; set; }

        public virtual IList<DataPointToAuction> DataPoints { get; set; }

        public virtual DateTime Timestamp { get; set; }
    }

    public class Upgrade
    {
        public virtual Guid Id { get; set; }
        public virtual int UpgradeId { get; set; }
        public virtual Auction Auction { get; set; }
    }

    public class Modifier
    {
        public virtual Guid Id { get; set; }
        public virtual int TypeId { get; set; }
        public virtual int Value { get; set; }
        public virtual Auction Auction { get; set; }
    }

    public class AuctionMapping : ClassMap<Auction>
    {
        public AuctionMapping()
        {
            Id(a => a.Id).GeneratedBy.Assigned();

            Version(a => a.Timestamp);
            Map(a => a.ItemId).Not.Nullable();
            Map(a => a.PlacedBy).Not.Nullable().Length(31);
            Map(a => a.Realm).Not.Nullable().Length(31);
            Map(a => a.CurrentBid).Not.Nullable();
            Map(a => a.Buyout).Not.Nullable();
            Map(a => a.Quantity).Not.Nullable();
            Map(a => a.Random).Not.Nullable();
            Map(a => a.Seed).Not.Nullable();
            Map(a => a.GenerationContext).Not.Nullable();
            Map(a => a.PetSpeciesId).Nullable();
            Map(a => a.PetBreedId).Nullable();
            Map(a => a.PetLevel).Nullable();
            Map(a => a.PetQualityId).Nullable();

            HasMany(a => a.UpgradeList).Inverse().Cascade.AllDeleteOrphan();
            HasMany(a => a.Modifiers).Inverse().Cascade.AllDeleteOrphan();

            HasMany(a => a.DataPoints);
        }
    }

    public class UpgradeMapping : ClassMap<Upgrade>
    {
        public UpgradeMapping()
        {
            Id(a => a.Id).GeneratedBy.Guid();

            Map(a => a.UpgradeId).Not.Nullable();

            References(a => a.Auction);
        }
    }

    public class ModifierMapping : ClassMap<Modifier>
    {
        public ModifierMapping()
        {
            Id(a => a.Id).GeneratedBy.Guid();

            Map(a => a.TypeId).Not.Nullable();
            Map(a => a.Value).Not.Nullable();

            References(a => a.Auction);
        }
    }
}