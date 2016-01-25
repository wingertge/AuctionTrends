using System.Windows;
using System.Windows.Media;

namespace AuctionTrends.Util
{
    public class QualityBrush
    {
        public static Brush FromQuality(int quality)
        {
            switch (quality)
            {
                case 0:
                    return (SolidColorBrush) new BrushConverter().ConvertFrom("#9d9d9d");
                case 1:
                    return Brushes.White;
                case 2:
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#1eff00");
                case 3:
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#0070dd");
                case 4:
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#a335ee");
                case 5:
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#ff8000");
                case 6:
                case 7:
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#e5cc80");
                default:
                    return Brushes.White;
            }
        }
    }
}