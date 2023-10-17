using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReferencePoints.Converters;

public class InvertedBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = (bool)value;

        return flag ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (Visibility)value == Visibility.Collapsed;
    }
}