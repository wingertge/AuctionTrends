using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AuctionTrends.Common.Models;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace AuctionTrends
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISessionFactory _sessionFactory;

        private Graph _graphPage = null;
        private Graph Graph => _graphPage ?? (_graphPage = new Graph(_sessionFactory));

        public MainWindow()
        {
            InitializeComponent();

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            _sessionFactory = CreateSessionFactory();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(functionFrame == null) return;
            switch ((string)((ComboBoxItem)functionSelect.SelectedItem).Content)
            {
                case "Graph":
                    functionFrame.Navigate(Graph);
                    return;
                default:
                    functionFrame.Content = "";
                    return;
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(
                    MsSqlConfiguration.MsSql2012.ConnectionString(a => a.FromConnectionStringWithKey("connectionString")))
                .Mappings(a =>
                {
                    a.FluentMappings.Add<ItemDataMapping>();
                    a.FluentMappings.Add<ItemMapping>();
                })
                .ExposeConfiguration(a => a.SetProperty(NHibernate.Cfg.Environment.CommandTimeout, (TimeSpan.FromMinutes(5).TotalSeconds).ToString(CultureInfo.InvariantCulture)))
                //.ExposeConfiguration(a => new SchemaExport(a).Create(false, true))
                .BuildSessionFactory();
        }
    }
}
