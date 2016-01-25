using FluentNHibernate.Mapping;

namespace AuctionTrends.Server.Models.File
{
    public class Auction
    {
        public virtual long AuctionId { get; set; }
        public virtual long ItemId { get; set; }
        public virtual long Buyout { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string RealmSlug { get; set; }
        public virtual string LastTimeLeft { get; set; }
        public virtual int? PetSpeciesId { get; set; }
        public virtual int? PetBreedId { get; set; }
        public virtual int? PetLevel { get; set; }
        public virtual int? PetQualityId { get; set; }
    }

    public class AuctionMapping : ClassMap<Auction>
    {
        public AuctionMapping()
        {
            Table("running_auctions");

            Id(a => a.AuctionId).GeneratedBy.Assigned();
            Map(a => a.ItemId);
            Map(a => a.Buyout);
            Map(a => a.Quantity);
            Map(a => a.RealmSlug);
            Map(a => a.LastTimeLeft);
            Map(a => a.PetSpeciesId);
            Map(a => a.PetBreedId);
            Map(a => a.PetLevel);
            Map(a => a.PetQualityId);
        }
    }
}