using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HMI.Converter
{
    [ValueConversion(typeof(object), typeof(Color))]
    public class IntToLinearGradientBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short)
            {
                switch((short)value)
                {
                    case 0: return (System.Windows.Media.Brush)Application.Current.FindResource("FP_Gray_Gradient");
                    case 1: return (System.Windows.Media.Brush)Application.Current.FindResource("FP_Yellow_Gradient");
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
