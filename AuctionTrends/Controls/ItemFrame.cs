using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using AuctionTrends.Common.Models;
using AuctionTrends.Util;

namespace AuctionTrends.Controls
{
    public class ItemFrame : StackPanel
    {
        private readonly Border _iconBorder = new Border
        {
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Black,
            Width = 56,
            Height = 56,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Padding = new Thickness(2)
        };
        private readonly CachedImage.Image _icon = new CachedImage.Image
        {
            ImageUrl = "",
            Width = 56,
            Height = 56,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };
        private readonly StackPanel _itemInfo = new StackPanel {Orientation = Orientation.Vertical, Width = 231, Height = 56};
        private readonly Label _itemName = new Label();
        private readonly OutlinedText _itemQuality = new OutlinedText();
        private readonly Label _itemDescription = new Label();
        private readonly Label _heroicTooltip = new Label();
        private readonly Label _itemLevel = new Label();
        private readonly Label _buyPrice = new Label();
        private readonly Label _sellPrice = new Label();
        private readonly StackPanel _itemHeader = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        private readonly StackPanel _itemPower = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        private readonly StackPanel _itemValue = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

        private Item _item;

        
        public Item Item { get { return _item; } set { _item = value; UpdateInfo(); } }

        public ItemFrame()
        {
            Width = 293;
            Height = 60;
            Orientation = Orientation.Horizontal;
            UpdateInfo();
        }

        

        private void UpdateInfo()
        {
            if (Item == null)
            {
                _icon.ImageUrl = "";
                _itemHeader.Children.Clear();
                _itemPower.Children.Clear();
                _itemValue.Children.Clear();
                _itemInfo.Children.Clear();
                return;
            }
            Children.Clear();
            _itemHeader.Children.Clear();
            _itemPower.Children.Clear();
            _itemValue.Children.Clear();
            _itemInfo.Children.Clear();


            _icon.ImageUrl = $"http://us.media.blizzard.com/wow/icons/56/{Item.Icon}.jpg";
            _iconBorder.Child = _icon;
            _itemName.Content = Item.Name;
            _itemName.VerticalAlignment = VerticalAlignment.Center;
            _itemName.Padding = new Thickness(0, 0, 2, 0);
            _itemQuality.Text = GetQualityText(Item.Quality);
            _itemQuality.Fill = QualityBrush.FromQuality(Item.Quality);
            _itemQuality.Stroke = Brushes.Black;
            _itemQuality.FontSize = 12;
            _itemQuality.StrokeThickness = 1;
            _itemQuality.VerticalAlignment = VerticalAlignment.Center;
            _itemQuality.Bold = true;
            _itemDescription.Content = Item.Description;
            _itemDescription.Padding = new Thickness(0);
            _itemDescription.Margin = new Thickness(-1);
            _itemDescription.HorizontalAlignment = HorizontalAlignment.Center;
            _heroicTooltip.Content = "Heroic";
            _heroicTooltip.Foreground = Brushes.LimeGreen;
            _heroicTooltip.Visibility = Item.HeroicTooltip ? Visibility.Visible : Visibility.Collapsed;
            _heroicTooltip.Padding = new Thickness(0);
            _heroicTooltip.Margin = new Thickness(1, -2, -2, -2);
            _itemLevel.Content = "iLvl"+Item.ItemLevel;
            _itemLevel.Padding = new Thickness(0);
            _itemLevel.Margin = new Thickness(-2, -2, 1, -2);
            _buyPrice.Content = "Buy Price: " + new GoldValue(Item.BuyPrice);
            _buyPrice.Padding = new Thickness(0);
            _buyPrice.Margin = new Thickness(-2, -2, 2, -2);
            _sellPrice.Content = "Sell Price: " + new GoldValue(Item.SellPrice);
            _sellPrice.Padding = new Thickness(0);
            _sellPrice.Margin = new Thickness(2, -2, -2, -2);

            _itemHeader.Children.Add(_itemName);
            _itemHeader.Children.Add(_itemQuality);
            _itemHeader.MouseLeftButtonUp += ItemHeaderClicked;

            _itemPower.Children.Add(_itemLevel);
            _itemPower.Children.Add(_heroicTooltip);

            _itemValue.Children.Add(_buyPrice);
            _itemValue.Children.Add(_sellPrice);

            _itemInfo.Children.Add(_itemHeader);
            _itemInfo.Children.Add(_itemPower);
            _itemInfo.Children.Add(_itemValue);
            _itemInfo.Children.Add(_itemDescription);

            Children.Add(_iconBorder);
            Children.Add(_itemInfo);
        }

        private void ItemHeaderClicked(object sender, MouseButtonEventArgs e)
        {
            Process.Start($"http://www.wowhead.com/item={Item.Id}");
        }

        private static string GetQualityText(int quality)
        {
            switch (quality)
            {
                case 0:
                    return "Poor";
                case 1:
                    return "Common";
                case 2:
                    return "Uncommon";
                case 3:
                    return "Rare";
                case 4:
                    return "Epic";
                case 5:
                    return "Legendary";
                case 6:
                    return "Artifact";
                case 7:
                    return "Heirloom";
                default:
                    return "None";
            }
        }
    }
}