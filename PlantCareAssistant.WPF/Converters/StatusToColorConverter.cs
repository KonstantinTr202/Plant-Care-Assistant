using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PlantCareAssistant.WPF.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "здоровое" => new SolidColorBrush(Colors.Green),
                    "требует внимания" => new SolidColorBrush(Colors.Orange),
                    "болеет" => new SolidColorBrush(Colors.Red),
                    "погибло" => new SolidColorBrush(Colors.Gray),
                    _ => new SolidColorBrush(Colors.Black)
                };
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}