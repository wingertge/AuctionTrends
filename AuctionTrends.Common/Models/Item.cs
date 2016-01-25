using System;
using FluentNHibernate.Mapping;

namespace AuctionTrends.Common.Models
{
    public class Item
    {
        public virtual int Id { get; set; }
        public virtual string Icon { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual string Description { get; set; }
        public virtual string Name { get; set; }
        public virtual long BuyPrice { get; set; }
        public virtual int ItemLevel { get; set; }
        public virtual int Quality { get; set; }
        public virtual int SellPrice { get; set; }
        public virtual string NameDescription { get; set; }
        public virtual bool Upgradable { get; set; }
        public virtual bool HeroicTooltip { get; set; }
    }

    public class ItemMapping : ClassMap<Item>
    {
        public ItemMapping()
        {
            Id(a => a.Id).GeneratedBy.Assigned();
            Version(a => a.CreatedAt);

            Map(a => a.Icon);
            Map(a => a.Description).Length(1024);
            Map(a => a.Name);
            Map(a => a.BuyPrice);
            Map(a => a.ItemLevel);
            Map(a => a.Quality);
            Map(a => a.SellPrice);
            Map(a => a.NameDescription).Length(1024);
            Map(a => a.Upgradable);
            Map(a => a.HeroicTooltip);
        }
    }
}