using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HMI.Converter
{
    [ValueConversion(typeof(object), typeof(Color))]
    public class IntToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short)
            {
                switch((short)value)
                {
                    case 0: return (Color)ColorConverter.ConvertFromString("#FF807F7F"); //gray
                    case 1: return (Color)ColorConverter.ConvertFromString("#FF12C312"); //light green
                    case 2: return (Color)ColorConverter.ConvertFromString("#FF807F7F"); //gray
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
