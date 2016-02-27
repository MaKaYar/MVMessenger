﻿using System;
using System.Windows;
using System.Windows.Data;

namespace MessengerClient.ViewModel.ValueConverters
{

    [ValueConversion(typeof(bool), typeof(HorizontalAlignment))]
    public class FromMeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return HorizontalAlignment.Right;
            else
                return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (object)value;
        }
    }
}