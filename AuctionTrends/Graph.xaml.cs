using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AuctionTrends.Common.Models;
using AuctionTrends.Controls;
using DevExpress.Xpf.Charts;
using NHibernate;
using NHibernate.Linq;

namespace AuctionTrends
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : Page
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly Dictionary<long, List<ItemData>> _dataCache = new Dictionary<long, List<ItemData>>();
        private DateTime _lastCompleteQuery;
        private readonly List<SeriesPoint> _adjustedMeanSeries = new List<SeriesPoint>();
        private readonly List<SeriesPoint> _meanSeries = new List<SeriesPoint>();
        private readonly List<SeriesPoint> _lowValueSeries = new List<SeriesPoint>();
        private readonly List<SeriesPoint> _highValueSeries = new List<SeriesPoint>();
        private readonly List<SeriesPoint> _quantitySeries = new List<SeriesPoint>();

        public Graph(ISessionFactory sessionFactory)
        {
            InitializeComponent();
            _sessionFactory = sessionFactory;
            adjustedMeanCheck.Foreground = new SolidColorBrush(chart.Palette[0]);
            meanCheck.Foreground = new SolidColorBrush(chart.Palette[1]);
            lowValueCheck.Foreground = new SolidColorBrush(chart.Palette[2]);
            highValueCheck.Foreground = new SolidColorBrush(chart.Palette[3]);
            quantityCheck.Foreground = new SolidColorBrush(chart.Palette[4]);

            adjustedMeanCheck.Checked += (a, b) => adjustedMeanSeries.Points.AddRange(_adjustedMeanSeries);
            adjustedMeanCheck.Unchecked += (a, b) => adjustedMeanSeries.Points.Clear();
            meanCheck.Checked += (a, b) => meanSeries.Points.AddRange(_meanSeries);
            meanCheck.Unchecked += (a, b) => meanSeries.Points.Clear();
            lowValueCheck.Checked += (a, b) => lowestSeries.Points.AddRange(_lowValueSeries);
            lowValueCheck.Unchecked += (a, b) => lowestSeries.Points.Clear();
            highValueCheck.Checked += (a, b) => highestSeries.Points.AddRange(_highValueSeries);
            highValueCheck.Unchecked += (a, b) => highestSeries.Points.Clear();
            quantityCheck.Checked += (a, b) => quantitySeries.Points.AddRange(_quantitySeries);
            quantityCheck.Unchecked += (a, b) => quantitySeries.Points.Clear();
        }

        private async void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text.Length < 1)
            {
                HideSearchResults();
                _lastCompleteQuery = DateTime.Now;
                return;
            }
            DisplayLoadingSearch();

            var search = searchBox.Text;
            await Task.Run(() => LoadItems(search));
        }

        private void search_LostFocus(object sender, RoutedEventArgs e)
        {
            HideSearchResults();
        }

        private void HideSearchResults()
        {
            searchLoading.Visibility = Visibility.Visible;
            searchResult1.Visibility = Visibility.Collapsed;
            searchResult2.Visibility = Visibility.Collapsed;
            searchResult3.Visibility = Visibility.Collapsed;
            searchResult4.Visibility = Visibility.Collapsed;
            searchResult5.Visibility = Visibility.Collapsed;
            searchResultsPanel.Visibility = Visibility.Collapsed;
        }

        private void LoadItems(string searchText)
        {
            var started = DateTime.Now;
            var items = new List<Item>();
            searchText = searchText.Trim();
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    items =
                        session.Query<Item>()
                            .Where(a => a.Name == searchText)
                            .OrderBy(a => a.Id)
                            .Take(5).ToList();
                    if (items.Count < 5)
                    {
                        var ids = items.Select(b => b.Id).ToList();
                        items.AddRange(
                            session.Query<Item>()
                                .Where(a => a.Name.StartsWith(searchText + " ") && !ids.Contains(a.Id))
                                .OrderBy(a => a.Id)
                                .Take(5 - items.Count)
                                .ToList());
                    }
                    if (items.Count < 5)
                    {
                        var ids = items.Select(b => b.Id).ToList();
                        items.AddRange(
                            session.Query<Item>()
                                .Where(
                                    a =>
                                        (a.Name.EndsWith(" " + searchText) ||
                                         a.Name.Contains(" " + searchText + " ")) &&
                                        !ids.Contains(a.Id))
                                .OrderBy(a => a.Id)
                                .Take(5 - items.Count)
                                .ToList());
                    }
                    if (items.Count < 5)
                    {
                        var ids = items.Select(b => b.Id).ToList();
                        items.AddRange(
                            session.Query<Item>()
                                .Where(a => a.Name.StartsWith(searchText) && !ids.Contains(a.Id))
                                .OrderBy(a => a.Id)
                                .Take(5 - items.Count)
                                .ToList());
                    }
                    if (items.Count < 5)
                    {
                        var ids = items.Select(b => b.Id).ToList();
                        items.AddRange(
                            session.Query<Item>()
                                .Where(a => a.Name.Contains(searchText) && !ids.Contains(a.Id))
                                .OrderBy(a => a.Id)
                                .Take(5 - items.Count)
                                .ToList());
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            if (_lastCompleteQuery >= started) return;
            _lastCompleteQuery = started;
            Dispatcher.Invoke(() => DisplaySearch(items));
        }

        private void DisplaySearch(IReadOnlyList<Item> items)
        {
            HideSearchResults();
            if (items.Count == 0)
            {
                return;
            }
            searchLoading.Visibility = Visibility.Collapsed;
            searchResult1.Visibility = Visibility.Visible;
            searchResult1.Item = items[0];
            if (items.Count > 1)
            {
                searchResult2.Visibility = Visibility.Visible;
                searchResult2.Item = items[1];
            }
            if (items.Count > 2)
            {
                searchResult3.Visibility = Visibility.Visible;
                searchResult3.Item = items[2];
            }
            if (items.Count > 3)
            {
                searchResult4.Visibility = Visibility.Visible;
                searchResult4.Item = items[3];
            }
            if (items.Count > 4)
            {
                searchResult5.Visibility = Visibility.Visible;
                searchResult5.Item = items[4];
            }
            searchResultsPanel.Visibility = Visibility.Visible;
        }

        private void DisplayLoadingSearch()
        {
            searchLoading.Visibility = Visibility.Visible;
            searchResult1.Visibility = Visibility.Collapsed;
            searchResult2.Visibility = Visibility.Collapsed;
            searchResult3.Visibility = Visibility.Collapsed;
            searchResult4.Visibility = Visibility.Collapsed;
            searchResult5.Visibility = Visibility.Collapsed;
            searchResultsPanel.Visibility = Visibility.Visible;
        }

        private async void searchResult_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var result = (ItemFrame) sender;
            selectedItem.Item = result.Item;
            HideSearchResults();
            Focus();
            chart.Visibility = Visibility.Collapsed;
            emptyGraphPanel.Visibility = Visibility.Collapsed;
            loadingPanel.Visibility = Visibility.Visible;
            var selectedItemId = selectedItem.Item.Id;
            if (!_dataCache.ContainsKey(selectedItem.Item.Id))
            {
                await Task.Run(() =>
                {
                    using (var session = _sessionFactory.OpenSession())
                    using (var transaction = session.BeginTransaction())
                    {
                        try
                        {
                            var dataPoints =
                                session.Query<ItemData>()
                                    .Where(a => a.Id == selectedItem.Item.Id)
                                    .ToList();
                            transaction.Commit();
                            _dataCache[selectedItem.Item.Id] = dataPoints;
                        }
                        catch (Exception)
                        {
                            if(transaction.IsActive)
                                transaction.Rollback();
                        }
                    }
                });
            }
            if(selectedItemId != selectedItem.Item.Id)
                return;
            var earliestDatapoint = _dataCache[selectedItem.Item.Id].Select(a => a.TimeStamp)
                .OrderBy(a => a)
                .FirstOrDefault();
            startDate.DisplayDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(earliestDatapoint);
            loadingStatus.Content = "Generating graph...";
            await Task.Run(() => UpdateGraph());
            loadingPanel.Visibility = Visibility.Collapsed;
            loadingStatus.Content = "Loading data...";
            chart.Visibility = Visibility.Visible;
        }

        private void UpdateGraph()
        {
            //TODO: Average Length and market values

            var dataPoints = _dataCache[selectedItem.Item.Id];

            _adjustedMeanSeries.Clear();
            _adjustedMeanSeries.AddRange(dataPoints.Select(a => new SeriesPoint(a.SellDate, a.AdjustedMeanValue)));

            _meanSeries.Clear();
            _meanSeries.AddRange(dataPoints.Select(a => new SeriesPoint(a.SellDate, a.MeanValue)));

            _highValueSeries.Clear();
            _highValueSeries.AddRange(dataPoints.Select(a => new SeriesPoint(a.SellDate, a.HighestValue)));

            _lowValueSeries.Clear();
            _lowValueSeries.AddRange(dataPoints.Select(a => new SeriesPoint(a.SellDate, a.LowestValue)));

            _quantitySeries.Clear();
            _quantitySeries.AddRange(dataPoints.Select(a => new SeriesPoint(a.SellDate, a.Quantity)));

            Dispatcher.Invoke(() =>
            {
                adjustedMeanSeries.Points.Clear();
                meanSeries.Points.Clear();
                lowestSeries.Points.Clear();
                highestSeries.Points.Clear();
                if(adjustedMeanCheck.IsChecked.HasValue && adjustedMeanCheck.IsChecked.Value) adjustedMeanSeries.Points.AddRange(_adjustedMeanSeries);
                if (meanCheck.IsChecked.HasValue && meanCheck.IsChecked.Value) meanSeries.Points.AddRange(_meanSeries);
                if (lowValueCheck.IsChecked.HasValue && lowValueCheck.IsChecked.Value) lowestSeries.Points.AddRange(_lowValueSeries);
                if (highValueCheck.IsChecked.HasValue && highValueCheck.IsChecked.Value) highestSeries.Points.AddRange(_highValueSeries);
                if (quantityCheck.IsChecked.HasValue && quantityCheck.IsChecked.Value) quantitySeries.Points.AddRange(_quantitySeries);
            });
        }

        private void searchResult_MouseLeave(object sender, MouseEventArgs e)
        {
            var panel = (StackPanel)sender;
            panel.Background = SystemColors.WindowBrush;
        }

        private void searchResult_MouseEnter(object sender, MouseEventArgs e)
        {
            var panel = (StackPanel) sender;
            panel.Background = SystemColors.HighlightBrush;
        }
    }
}
