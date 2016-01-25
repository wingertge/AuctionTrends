using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace AuctionTrends.Common.Models
{
    public class DataPoint
    {
        public virtual string RealmName { get; set; }
        public virtual string RealmSlug { get; set; }
        public virtual double TimeStamp { get; set; }
        public virtual IList<DataPointToAuction> Auctions { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is DataPoint)) return false;
            var dataPoint = (DataPoint) obj;
            return TimeStamp == dataPoint.TimeStamp && RealmSlug == dataPoint.RealmSlug;
        }

        public override int GetHashCode()
        {
            return (TimeStamp + RealmName).GetHashCode();
        }
    }

    public class DataPointMapping : ClassMap<DataPoint>
    {
        public DataPointMapping()
        {
            CompositeId().KeyProperty(a => a.RealmSlug, a => a.Length(32)).KeyProperty(a => a.TimeStamp);

            Map(x => x.RealmName).Not.Nullable().Length(32);
            HasMany(x => x.Auctions)
                .Inverse()
                .KeyColumns.Add("DataPointRealmSlug", "DataPointTimeStamp").Cascade.AllDeleteOrphan();
        }
    }
}