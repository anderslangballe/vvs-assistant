﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace VVSAssistant.ValueConverters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime))
                return value;
            return ((DateTime) value).ToString("dd. MMM yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
