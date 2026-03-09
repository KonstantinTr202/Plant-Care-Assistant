using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PlantCareAssistant.WPF.Converters
{
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "здоровое" => new SolidColorBrush(Color.FromRgb(230, 250, 230)),
                    "требует внимания" => new SolidColorBrush(Color.FromRgb(255, 244, 230)),
                    "болеет" => new SolidColorBrush(Color.FromRgb(255, 230, 230)),
                    "погибло" => new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                    _ => new SolidColorBrush(Colors.White)
                };
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}