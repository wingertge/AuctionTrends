using System;
using FluentNHibernate.Mapping;

namespace AuctionTrends.Common.Models
{
    public class ItemData
    {
        protected bool Equals(ItemData other)
        {
            return Id == other.Id && TimeStamp.Equals(other.TimeStamp) && string.Equals(RealmSlug, other.RealmSlug) && MeanValue.Equals(other.MeanValue) && AdjustedMeanValue.Equals(other.AdjustedMeanValue) && LowestValue.Equals(other.LowestValue) && HighestValue.Equals(other.HighestValue) && Quantity == other.Quantity;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ TimeStamp.GetHashCode();
                hashCode = (hashCode*397) ^ (RealmSlug?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ MeanValue.GetHashCode();
                hashCode = (hashCode*397) ^ AdjustedMeanValue.GetHashCode();
                hashCode = (hashCode*397) ^ LowestValue.GetHashCode();
                hashCode = (hashCode*397) ^ HighestValue.GetHashCode();
                hashCode = (hashCode*397) ^ Quantity;
                return hashCode;
            }
        }

        public virtual long Id { get; set; }
        public virtual double TimeStamp { get; set; }
        public virtual string RealmSlug { get; set; }
        public virtual double MeanValue { get; set; }
        public virtual double AdjustedMeanValue { get; set; }
        public virtual double LowestValue { get; set; }
        public virtual double HighestValue { get; set; }
        public virtual int Quantity { get; set; }
        public virtual DateTime SellDate => new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(TimeStamp);

        public override bool Equals(object data)
        {
            if (!(data is ItemData)) return false;
            return Equals((ItemData) data);
        }
    }

    public class ItemDataMapping : ClassMap<ItemData>
    {
        public ItemDataMapping()
        {
            CompositeId().KeyProperty(a => a.Id).KeyProperty(a => a.TimeStamp).KeyProperty(a => a.RealmSlug);
            Map(a => a.MeanValue);
            Map(a => a.AdjustedMeanValue);
            Map(a => a.LowestValue);
            Map(a => a.HighestValue);
            Map(a => a.Quantity);
        }
    }
}