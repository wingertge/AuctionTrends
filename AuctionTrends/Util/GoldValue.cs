namespace AuctionTrends.Util
{
    public class GoldValue
    {
        private readonly long _copperValue;

        public GoldValue(long copperValue)
        {
            _copperValue = copperValue;
        }

        public GoldValue(int gold, int silver, int copper)
        {
            _copperValue = gold*10000 + silver*100 + copper;
        }

        public override string ToString()
        {
            var goldValue = (int) (_copperValue/10000);
            var silverValue = ((int) (_copperValue/100)) - goldValue * 100;
            var copperValue = _copperValue - silverValue*100 - goldValue*10000;
            var result = $"{copperValue}c";
            if (silverValue > 0) result = $"{silverValue}s " + result;
            if (goldValue > 0) result = $"{goldValue}g " + result;
            return result;
        }

        public static GoldValue FromGold(int gold)
        {
            return new GoldValue(gold, 0, 0);
        }

        public static GoldValue FromSilver(int silver)
        {
            return new GoldValue(0, silver, 0);
        }

        public static GoldValue FromCopper(int copper)
        {
            return new GoldValue(0, 0, copper);
        }
    }
}