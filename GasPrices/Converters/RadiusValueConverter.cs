using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace GasPrices.Converters;

public class RadiusValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isValid = int.TryParse(value?.ToString(), out var radiusInt);

        return isValid ? radiusInt : 1;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value!.ToString();
    }
}