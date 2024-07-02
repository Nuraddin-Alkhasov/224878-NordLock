using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI.Converter
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class BoolToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                    return (System.Windows.Media.Brush)Application.Current.FindResource("FP_Yellow_Gradient"); 
                else
                    return (System.Windows.Media.Brush)Application.Current.FindResource("FP_LightGreen_Gradient");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
