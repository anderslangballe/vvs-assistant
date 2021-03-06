﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace VVSAssistant.ValueConverters
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double)) return value;
            return $"kr. {((double) value):n2}"; // new CultureInfo("da-DK")
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
