using System;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using AuctionTrends.Common.Models;
using AuctionTrends.Server.Models.Json;
using AuctionTrends.Server.Settings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

namespace AuctionTrends.Server
{
    public partial class MainWindow
    {
        private readonly TimerPlus _timer;
        private readonly Timer _infoTimer;
        private readonly ISessionFactory _sessionFactory;
        private double _lastTimeStamp = -1;
        private int _totalRecords;

        public MainWindow()
        {
            InitializeComponent();

            NHibernateProfiler.Initialize();

            apiKeyTextBox.Text = MainSettings.Default.APIKey;
            realmsList.Items.AddRange(MainSettings.Default.Servers.ToList());

            _sessionFactory = CreateSessionFactory();
   
            _timer = new TimerPlus {Interval = 600000};
            _timer.Elapsed += (a, b) => LoadData();

            _infoTimer = new Timer(1000);
            _infoTimer.Elapsed += (a, b) => UpdateInfo(_lastTimeStamp, _totalRecords, _timer.Enabled ? (int)(_timer.TimeLeft / 1000) : -1);

            _infoTimer.Start();
            _timer.Start();

            //LoadData();
        }

        private void AddButtonClicked(object sender, RoutedEventArgs e)
        {
            var popup = new AddRealmPopup(this);
            popup.Show();
        }

        private void RemoveButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedClone = realmsList.SelectedItems.Clone();
            realmsList.Items.RemoveAll(selectedClone);
            MainSettings.Default.Servers.RemoveAll(selectedClone);
            MainSettings.Default.Save();
        }

        public void AddRealm(string region, string name)
        {
            var serverString = $"{region}:{name}";
            realmsList.Items.Add(serverString);
            MainSettings.Default.Servers.Add(serverString);
            MainSettings.Default.Save();
        }

        private void ApiKeyChanged(object sender, TextChangedEventArgs e)
        {
            MainSettings.Default.APIKey = apiKeyTextBox.Text;
            MainSettings.Default.Save();
        }

        private void UpdateInfo(double lastCheckTime, int totalDataPoints, int timeToNextQuery)
        {
            var lastCheckedDate = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(lastCheckTime).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt");
            if (_lastTimeStamp == -1) lastCheckedDate = "N/A";
            string nextQueryFormatted;
            if (timeToNextQuery != -1)
            {
                var ttQTimeSpan = new TimeSpan(0, 0, timeToNextQuery);
                nextQueryFormatted = $"{ttQTimeSpan.Minutes}m {ttQTimeSpan.Seconds}s";
            }
            else nextQueryFormatted = "Fetching...";
            Dispatcher.Invoke(() => infoLabel.Content =
                $"Last Auction Data Time: {lastCheckedDate} | Total Data Points: {totalDataPoints} | Next Query: {nextQueryFormatted}");
        }

        private async void LoadData()
        {
            _timer.Stop();

            try
            {
                using (var webClient = new WebClient())
                {
                    foreach (var server in MainSettings.Default.Servers)
                    {
                        var dataPointJson = webClient.DownloadString(BuildUrl(server));
                        dynamic dataPointObject = JObject.Parse(dataPointJson);
                        string url = dataPointObject.files[0].url;
                        double timestamp = dataPointObject.files[0].lastModified;

                        var auctionDataJson = await webClient.DownloadStringTaskAsync(new Uri(url));
                        var auctionData = JsonConvert.DeserializeObject<RootObject>(auctionDataJson);
                        Models.Util.InterpretJsonModel(timestamp, server, auctionData, _sessionFactory, this);
                    }

                    using (var session = _sessionFactory.OpenSession())
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            try
                            {
                                var lastCheck = session.Query<ItemData>().Max(a => (double?) a.TimeStamp);
                                if (lastCheck.HasValue)
                                {
                                    var dataPoints =
                                        session.QueryOver<ItemData>()
                                            .Select(Projections.CountDistinct("TimeStamp"))
                                            .FutureValue<int>()
                                            .Value;
                                    _lastTimeStamp = lastCheck.Value;
                                    _totalRecords = dataPoints;
                                }

                                transaction.Commit();
                                UpdateInfo(_lastTimeStamp, _totalRecords, (int) (_timer.TimeLeft/1000));
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                ShowError(e);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }

            _timer.Start();
        }

        private static string BuildUrl(string server)
        {
            var region = server.Split(':')[0];
            var serverName = server.Split(':')[1];
            serverName = serverName.Replace(" ", "%20");
            return $"https://{region.ToLower()}.api.battle.net/wow/auction/data/{serverName}?locale=en_GB&apikey={MainSettings.Default.APIKey}";
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(
                    MsSqlConfiguration.MsSql2012.ConnectionString(a => a.FromConnectionStringWithKey("connectionString")))
                .Mappings(a =>
                {
                    a.FluentMappings.Add<ItemDataMapping>();
                    a.FluentMappings.Add<Models.File.AuctionMapping>();
                })
                .ExposeConfiguration(a => a.SetProperty(Environment.CommandTimeout, (TimeSpan.FromMinutes(5).TotalSeconds).ToString(CultureInfo.InvariantCulture)))
                //.ExposeConfiguration(a => new SchemaExport(a).Create(false, true))
                .BuildSessionFactory();
        }

        public void ShowError(Exception e)
        {
            Dispatcher.Invoke(() => new ErrorWindow(e).Show());
        }
    }
}
