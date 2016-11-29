﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VVSAssistant.ValueConverters
{
    public class LetterToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch (value.ToString())
                {
                    case "A+++":
                        return new SolidColorBrush(Color.FromArgb(0, 0, 150, 69));
                    case "A++":
                        return new SolidColorBrush(Color.FromArgb(0, 80, 180, 74));
                    case "A+":
                        return new SolidColorBrush(Color.FromArgb(0, 157, 210, 79));
                    case "A":
                        return new SolidColorBrush(Color.FromArgb(0, 255, 255, 0));
                    case "B":
                        return new SolidColorBrush(Color.FromArgb(0, 255, 192, 0));
                    case "C":
                        return new SolidColorBrush(Color.FromArgb(0, 255, 102, 0));
                    case "D":
                    case "E":
                    case "F":
                    case "G":
                        return new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
                }
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
