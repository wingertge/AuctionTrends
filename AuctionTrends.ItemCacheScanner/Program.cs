using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AuctionTrends.Common.Models;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Linq;
using Environment = NHibernate.Cfg.Environment;

namespace AuctionTrends.ItemCacheScanner
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient();
        private static ISessionFactory _sessionFactory;

        private static async Task<Item> LoadDataAsync(int itemId)
        {
            try
            {
                var response = await Client.GetAsync($"http://us.battle.net/api/wow/item/{itemId}").ConfigureAwait(false);
                if(response.StatusCode == HttpStatusCode.ServiceUnavailable) throw new InvalidOperationException($"Daily limit exceeded, resume at {itemId - itemId % 5000}");
                if (response.StatusCode != HttpStatusCode.OK) return null;
                var responseValue = await response.Content.ReadAsStringAsync();
            
                var item = JsonConvert.DeserializeObject<Item>(responseValue);
                return item;
            }
            catch (Exception e)
            {
                
                throw;
            }
        }
        
        static void Main(string[] args)
        {
            NHibernateProfiler.Initialize();
            _sessionFactory = CreateSessionFactory();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine(ServicePointManager.DefaultConnectionLimit);
            var tasks = new List<Task<Item>>();
            
            for (var i = 1; i <= 500000; i++)
            { //For Item ID's 1 through 500000
                tasks.Add(LoadDataAsync(i));
                if (i%100 != 0) continue;
                Console.WriteLine($"Waiting for results {i - 100}-{i}");
                Task.WaitAll(tasks.ToArray());
                if (i%5000 != 0) continue;
                var items = tasks.Where(a => a.Result != null).Select(a => a.Result).ToList();
                var dbItems = ConvertItems(items, i-4999, i);
                    
                using (var session = _sessionFactory.OpenSession())
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.SetBatchSize(5000);
                        foreach (var dbItem in dbItems)
                            session.SaveOrUpdate(dbItem);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                tasks.Clear();
            }
            Task.WaitAll(tasks.ToArray());

            stopwatch.Stop();
            Console.WriteLine("Run Completed, Time elapsed: {0}", stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static IEnumerable<Common.Models.Item> ConvertItems(IEnumerable<Item> items, int startId, int endId)
        {
            var newItems = new List<Common.Models.Item>();
            var existingItems = new List<Common.Models.Item>();
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    existingItems = session.Query<Common.Models.Item>().Where(a => a.Id >= startId && a.Id <= endId).ToList();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            foreach (var item in items)
            {
                var newItem = new Common.Models.Item
                {
                    Id = item.Id,
                    BuyPrice = item.BuyPrice,
                    HeroicTooltip = item.HeroicTooltip,
                    ItemLevel = item.ItemLevel,
                    Name = item.Name,
                    NameDescription = item.NameDescription,
                    Quality = item.Quality,
                    SellPrice = item.SellPrice,
                    Upgradable = item.Upgradable,
                    Description = item.Description,
                    Icon = item.Icon
                };

                if (existingItems.Any(a => a.Id == item.Id))
                    newItem = existingItems.FirstOrDefault(a => a.Id == item.Id);

                newItems.Add(newItem);
            }

            return newItems;
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(
                    MsSqlConfiguration.MsSql2012.ConnectionString(a => a.FromConnectionStringWithKey("connectionString")))
                .Mappings(a =>
                {
                    a.FluentMappings.Add<ItemMapping>();
                })
                .ExposeConfiguration(a => a.SetProperty(Environment.CommandTimeout, (TimeSpan.FromMinutes(5).TotalSeconds).ToString(CultureInfo.InvariantCulture)))
                //.ExposeConfiguration(a => new SchemaExport(a).Create(false, true))
                .BuildSessionFactory();
        }
    }
}
