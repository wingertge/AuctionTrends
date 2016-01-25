using System.Collections.Generic;
using AuctionTrends.Common.Models;
using Newtonsoft.Json;

namespace AuctionTrends.ItemCacheScanner
{
    public class Item
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Stackable { get; set; }
        public int ItemBind { get; set; }
        public List<BonusStat> BonusStats { get; set; }
        public List<ItemSpell> ItemSpells { get; set; }
        public long BuyPrice { get; set; }
        public int ItemClass { get; set; }
        public int ItemSubClass { get; set; }
        public int ContainerSlots { get; set; }
        public int InventoryType { get; set; }
        public bool Equippable { get; set; }
        public int ItemLevel { get; set; }
        public int MaxCount { get; set; }
        public int MaxDurability { get; set; }
        public int MinFactionId { get; set; }
        public int MinReputation { get; set; }
        public int Quality { get; set; }
        public int SellPrice { get; set; }
        public int RequiredSkill { get; set; }
        public int RequiredLevel { get; set; }
        public int RequiredSkillRank { get; set; }
        public ItemSource ItemSource { get; set; }
        public int BaseArmor { get; set; }
        public bool HasSockets { get; set; }
        public bool IsAuctionable { get; set; }
        public int Armor { get; set; }
        public int DisplayInfoId { get; set; }
        public string NameDescription { get; set; }
        public string NameDescriptionColor { get; set; }
        public bool Upgradable { get; set; }
        public bool HeroicTooltip { get; set; }
        public string Context { get; set; }
        public List<int> BonusLists { get; set; }
        public List<string> AvailableContexts { get; set; }
        public BonusSummary BonusSummary { get; set; }
    }

    public class BonusStat
    {
        public int Stat { get; set; }
        public int Amount { get; set; }
    }

    public class BonusSummary
    {
        public List<string> DefaultBonusLists { get; set; }
        public List<int> ChanceBonusLists { get; set; }
        public List<BonusChance> BonusChances { get; set; }
    }

    public class BonusChance
    {
        public string ChanceType { get; set; }
        public Upgrade Upgrade { get; set; }
        public List<StatBonus> Stats { get; set; }
        public List<Socket> Sockets { get; set; } 
    }

    public class Socket
    {
        public string SocketType { get; set; }
    }

    public class StatBonus
    {
        public string StatId { get; set; }
        public int Delta { get; set; }
    }

    public class Upgrade
    {
        public string UpgradeType { get; set; }
        public int Id { get; set; }
    }

    public class ItemSource
    {
        public int SourceId { get; set; }
        public string SourceType { get; set; }
    }

    public class ItemSpell
    {
        public int SpellId { get; set; }
        public Spell Spell { get; set; }
        [JsonProperty("nCharges")]
        public int Charges { get; set; }
        public bool Consumable { get; set; }
        public int CategoryId { get; set; }
        public string Trigger { get; set; }
    }

    public class Spell
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subtext { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string CastTime { get; set; }
    }
}