using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GasPrices.Converters;

public class ListViewCalculateMaxHeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (double)value! * double.Parse(parameter!.ToString()!, CultureInfo.InvariantCulture);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}