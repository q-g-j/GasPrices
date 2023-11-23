using Avalonia;
using Avalonia.Controls.Primitives;
using GasPrices.Models;

namespace GasPrices.Views.CustomControls;

public class StationControl : TemplatedControl
{
    public static readonly StyledProperty<DisplayStation> StationProperty =
        AvaloniaProperty.Register<StationControl, DisplayStation>(nameof(Station));

    public DisplayStation Station
    {
        get => GetValue(StationProperty);
        set => SetValue(StationProperty, value);
    }
}