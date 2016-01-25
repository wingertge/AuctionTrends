using System;
using FluentNHibernate.Mapping;

namespace AuctionTrends.Common.Models
{
    public class DataPointToAuction
    {
        public virtual Guid Id { get; set; }
        public virtual DataPoint DataPoint { get; set; }
        public virtual Auction Auction { get; set; }
        public virtual string TimeLeft { get; set; }
    }

    public class DataPointToAuctionMapping : ClassMap<DataPointToAuction>
    {
        public DataPointToAuctionMapping()
        {
            Id(a => a.Id).GeneratedBy.Guid();

            Map(a => a.TimeLeft).Not.Nullable().Length(16);

            References(a => a.DataPoint).Columns("DataPointRealmSlug", "DataPointTimeStamp");
            References(a => a.Auction).Cascade.All();
        }
    }
}