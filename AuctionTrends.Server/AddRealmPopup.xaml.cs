using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AuctionTrends.Server
{
    /// <summary>
    /// Interaction logic for AddRealmPopup.xaml
    /// </summary>
    public partial class AddRealmPopup : Window
    {
        private readonly MainWindow _parent;

        public AddRealmPopup(MainWindow parent)
        {
            _parent = parent;
            InitializeComponent();
        }

        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            var region = "EU";
            switch ((string)((ListBoxItem)regionDropdown.SelectedValue).Content)
            {
                case "Europe":
                    region = "EU";
                    break;
                case "North America":
                    region = "US";
                    break;
                case "Korea":
                    region = "KR";
                    break;
                case "Taiwan":
                    region = "TW";
                    break;
            }

            _parent.AddRealm(region, realmNameTextBox.Text);
            Close();
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
