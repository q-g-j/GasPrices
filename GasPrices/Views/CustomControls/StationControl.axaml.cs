using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using BruTile.Wms;
using GasPrices.Models;

namespace GasPrices.Views.CustomControls;

public class StationControl : TemplatedControl
{
    private Border? _stationBorder;
    private Border? _stationDistanceBorder;

    public static readonly StyledProperty<DisplayStation> StationProperty =
        AvaloniaProperty.Register<StationControl, DisplayStation>(nameof(Station));

    public DisplayStation Station
    {
        get => GetValue(StationProperty);
        set => SetValue(StationProperty, value);
    }
}