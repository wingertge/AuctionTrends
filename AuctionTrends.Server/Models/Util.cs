using System;
using System.Collections.Generic;
using System.Linq;
using AuctionTrends.Common.Models;
using AuctionTrends.Server.Models.Json;
using NHibernate;
using NHibernate.Linq;

namespace AuctionTrends.Server.Models
{
    public static class Util
    {
        public static void InterpretJsonModel(double timeStamp, string realmSlug, RootObject root, ISessionFactory sessionFactory, MainWindow window)
        {
            var soldAuctions = new List<File.Auction>();
            var currentDataSet = root.Auctions;
            using (var session = sessionFactory.OpenSession())
            {
                session.SetBatchSize(25000);

                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var runningAuctions = session.Query<File.Auction>().Where(a => a.RealmSlug == realmSlug).ToList();
                        var toAdd = new List<File.Auction>();
                        var changed = new List<File.Auction>();
                        foreach (var auction in currentDataSet)
                        {
                            if (runningAuctions.All(a => a.AuctionId != auction.AuctionId))
                            {
                                toAdd.Add(new File.Auction
                                {
                                    AuctionId = auction.AuctionId,
                                    Buyout = auction.Buyout,
                                    ItemId = auction.ItemId,
                                    LastTimeLeft = auction.TimeLeft,
                                    PetBreedId = auction.PetBreedId,
                                    PetLevel = auction.PetLevel,
                                    PetQualityId = auction.PetQualityId,
                                    PetSpeciesId = auction.PetSpeciesId,
                                    Quantity = auction.Quantity,
                                    RealmSlug = realmSlug
                                });
                                continue;
                            }

                            var runningAuction = runningAuctions.FirstOrDefault(a => a.AuctionId == auction.AuctionId);
                            runningAuction.LastTimeLeft = auction.TimeLeft;
                            changed.Add(runningAuction);
                        }
                        var toCancel = new List<File.Auction>();
                        foreach (var auction in runningAuctions)
                        {
                            if (currentDataSet.Any(a => a.AuctionId == auction.AuctionId)) continue;
                            if (auction.LastTimeLeft != "SHORT")
                                soldAuctions.Add(auction);
                            toCancel.Add(auction);
                        }
                        foreach (var auction in toAdd)
                            session.Save(auction);
                        foreach (var auction in changed)
                            session.Update(auction);
                        foreach (var auction in toCancel)
                            session.Delete(auction);                       
                        
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        window.ShowError(e);
                    }
                }
                session.SetBatchSize(1);
            }

            var dataPoints = CompileItemData(timeStamp, realmSlug, soldAuctions);

            using (var session = sessionFactory.OpenSession())
            {
                session.SetBatchSize(25000);
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var dataPoint in dataPoints)
                            session.Save(dataPoint);

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        window.ShowError(e);
                    }
                }
                session.SetBatchSize(1);
            }
        }

        private static List<ItemData> CompileItemData(double timeStamp, string realmSlug, List<File.Auction> soldAuctions)
        {
            return (from i in new HashSet<long>(soldAuctions.Select(a => a.ItemId).ToList())
                where soldAuctions.Any(a => a.ItemId == i)
                let auctionsForItem = soldAuctions.Where(a => a.ItemId == i).ToList()
                let mean = (long) (auctionsForItem.Aggregate((long) 0, (a, b) => a + b.Buyout / b.Quantity)/auctionsForItem.Count)
                let totalCount = auctionsForItem.Aggregate(0, (a, b) => a + b.Quantity)
                let totalCountMultBuyout = auctionsForItem.Aggregate(0, (current, auction) => (int) (current + auction.Buyout))
                let adjustedMean = totalCountMultBuyout/totalCount
                let min = auctionsForItem.Select(a => a.Buyout / a.Quantity).Min()
                let max = auctionsForItem.Select(a => a.Buyout / a.Quantity).Max()
                select new ItemData
                {
                    Id = i, AdjustedMeanValue = adjustedMean, HighestValue = max, LowestValue = min, MeanValue = mean, Quantity = totalCount, RealmSlug = realmSlug, TimeStamp = timeStamp
                }).ToList();
        }
    }
}