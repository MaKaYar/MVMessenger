using System;
using System.Windows.Data;

namespace MessengerClient.ViewModel.ValueConverters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class TextMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Возвращаем строку в формате 123.456.789 руб.
            return (string) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (object) value;
        }
    }
}