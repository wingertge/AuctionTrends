using System.Collections.Generic;
using System.Linq;
using AuctionTrends.Common.Models.New;

namespace AuctionTrends.Analysis
{
    public static class Graph
    {
        public static double Median(List<Auction> auctions)
        {
            return auctions.Select(a => a.Buyout).Aggregate((a, b) => a + b)/
                   auctions.Select(a => a.Quantity).Aggregate((a, b) => a + b);
        }

        public static double MarketValue(List<Auction> auctions)
        {
            var mappedData = new Dictionary<double, int>();
            foreach (var auction in auctions)
            {
                var key = auction.Buyout / auction.Quantity;
                if (!mappedData.ContainsKey(key)) mappedData[key] = auction.Quantity;
                else mappedData[key] += auction.Quantity;
            }
            return mappedData.OrderByDescending(a => a.Value).FirstOrDefault().Key;
        }

        public static double ClusterHighEnd(List<Auction> dataPoints)
        {
            
            return 0;
        }

        public static double LowerMarketValue(List<Auction> dataPoints)
        {
            return 0;
        }

        public static double Min(List<Auction> dataPoints)
        {
            return dataPoints.Where(a => a.Buyout != 0).Select(a => a.Buyout / a.Quantity).OrderBy(a => a).FirstOrDefault();
        }

        public static double Max(List<Auction> dataPoints)
        {
            return dataPoints.Select(a => a.Buyout / a.Quantity).OrderByDescending(a => a).FirstOrDefault();
        }

        public static int Quantity(List<Auction> items)
        {
            return items.Select(a => a.Quantity).Aggregate((a, b) => a + b);
        }
    }
}