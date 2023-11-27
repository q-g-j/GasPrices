using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using GasPrices.Models;

namespace GasPrices.CustomControls;

public class StationCardControl : TemplatedControl
{
    public static readonly StyledProperty<DisplayStation> StationProperty =
        AvaloniaProperty.Register<StationControl, DisplayStation>(nameof(Station));

    public DisplayStation Station
    {
        get => GetValue(StationProperty);
        set => SetValue(StationProperty, value);
    }
}