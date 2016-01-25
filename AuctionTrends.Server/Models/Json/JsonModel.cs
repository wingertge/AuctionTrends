using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AuctionTrends.Server.Models.Json
{
    public class Realm
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
    }

    public class BonusList
    {
        [JsonProperty("bonusListId")]
        public int Id { get; set; }
    }

    public class Modifier
    {
        [JsonProperty("type")]
        public int TypeId { get; set; }
        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public class Auction
    {
        [JsonProperty("auc")]
        public long AuctionId { get; set; }
        [JsonProperty("item")]
        public long ItemId { get; set; }
        [JsonProperty("owner")]
        public string PlacedBy { get; set; }
        [JsonProperty("ownerRealm")]
        public string Realm { get; set; }
        [JsonProperty("bid")]
        public long CurrentBid { get; set; }
        [JsonProperty("buyout")]
        public long Buyout { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("timeLeft")]
        public string TimeLeft { get; set; }
        [JsonProperty("rand")]
        public long Random { get; set; }
        [JsonProperty("seed")]
        public long Seed { get; set; }
        [JsonProperty("context")]
        public int GenerationContext { get; set; }
        [JsonProperty("bonusLists")]
        public List<BonusList> UpgradeLists { get; set; }
        [JsonProperty("modifiers")]
        public List<Modifier> Modifiers { get; set; }
        [JsonProperty("petSpeciesId")]
        public int? PetSpeciesId { get; set; }
        [JsonProperty("petBreedId")]
        public int? PetBreedId { get; set; }
        [JsonProperty("petLevel")]
        public int? PetLevel { get; set; }
        [JsonProperty("petQualityId")]
        public int? PetQualityId { get; set; }
    }

    public class Realms : List<Realm>
    {
        
    }

    public class Auctions : List<Auction>
    {
        
    }

    public class RootObject
    {
        [JsonProperty("realms")]
        public Realms Realms { get; set; }
        [JsonProperty("auctions")]
        public Auctions Auctions { get; set; }
    } 
}