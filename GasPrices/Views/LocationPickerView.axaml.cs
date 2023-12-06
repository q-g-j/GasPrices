using System;
using Avalonia.Controls;
using GasPrices.Store;
using Mapsui.Tiling;
using ApiClients.Models;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Layers;
using Mapsui.Styles;
using System.Collections.Generic;
using System.Threading.Tasks;
using GasPrices.Extensions;
using Mapsui.Nts;
using Mapsui.UI.Avalonia.Extensions;
using GasPrices.ViewModels;
using Mapsui.Limiting;
using SettingsHandling;

namespace GasPrices.Views;

public partial class LocationPickerView : UserControl
{
    public LocationPickerView()
    {
        InitializeComponent();
    }

    public LocationPickerView(AppStateStore appStateStore, ISettingsReader settingsReader)
    {
        _appStateStore = appStateStore;
        _settingsReader = settingsReader;

        InitializeComponent();

        Initialize().FireAndForget();
    }

    private readonly AppStateStore? _appStateStore;
    private readonly ISettingsReader? _settingsReader;
    private GenericCollectionLayer<List<IFeature>>? _pinLayer;
    private MPoint? _cachedPoint;
    private MPoint? _cachedMapPoint;

    private void SetupMap()
    {
        MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        
        var germanyPanBounds = GetLimitsOfGermany();
        MapControl.Map.Layers.Add(CreatePanBoundsLayer(germanyPanBounds));
        MapControl.Map.Navigator.Limiter = new ViewportLimiterKeepWithinExtent();
        MapControl.Map.Navigator.OverridePanBounds = germanyPanBounds;
        
        MapControl.Map.Navigator.RotationLock = true;
        MapControl.UnSnapRotationDegrees = 30;
        MapControl.ReSnapRotationDegrees = 5;

        var pointLonLat = _cachedPoint ?? new MPoint(10.447683, 51.163361);

        _pinLayer = new GenericCollectionLayer<List<IFeature>>
        {
            Style = SymbolStyles.CreatePinStyle()
        };
        MapControl.Map.Layers.Add(_pinLayer);
        MapControl.Map.Layers[0].IsMapInfoLayer = false;

        _cachedMapPoint = SphericalMercator.FromLonLat(pointLonLat);

        MapControl.Map.Home += n =>
        {
            n.ZoomToBox(germanyPanBounds);

            if (_cachedPoint == null) return;
            
            var duration = 0;
            if (!OperatingSystem.IsBrowser())
            {
                duration = 1000;
            }
            n.CenterOnAndZoomTo(_cachedMapPoint!, 3, duration);
                
            PlacePin(_cachedMapPoint);
        };

        MapControl.Tapped += (_, e) =>
        {
            e.Handled = true;
            var tapPosition = e.GetPosition(MapControl).ToMapsui();
            var mapInfo = MapControl.GetMapInfo(tapPosition);

            if (mapInfo?.WorldPosition == null) return;
            PlacePin(mapInfo.WorldPosition!);

            var pos = SphericalMercator.ToLonLat(mapInfo.WorldPosition!);
            var coords = new Coords(pos.Y, pos.X);
            _appStateStore!.CoordsFromMapClient = coords;
            ((LocationPickerViewModel)DataContext!).ApplyButtonIsEnabled = true;
        };
    }

    private void PlacePin(MPoint mPoint)
    {
        _pinLayer?.Features.Clear();
        _pinLayer?.Features.Add(new GeometryFeature
        {
            Geometry = mPoint.ToPoint()
        });

        _pinLayer?.DataHasChanged();
    }

    private async Task Initialize()
    {
        var settings = await _settingsReader!.ReadAsync();
        if (settings is { LastKnownLatitude: not null, LastKnownLongitude: not null })
        {
            _cachedPoint = new MPoint(settings.LastKnownLongitude.Value, settings.LastKnownLatitude.Value);
        }
        else if (_appStateStore!.CoordsFromMapClient != null)
        {
            _cachedPoint = new MPoint(_appStateStore.CoordsFromMapClient.Longitude,
                _appStateStore.CoordsFromMapClient.Latitude);
        }

        SetupMap();
    }    
    
    private static MRect GetLimitsOfGermany()
    {
        var (minX, minY) = SphericalMercator.FromLonLat(5.866240, 47.270111);
        var (maxX, maxY) = SphericalMercator.FromLonLat(15.042050, 54.983104);

        // Increase the height to both sides
        const double extraHeight = 100000.0; // You can adjust this value as needed
        minY -= extraHeight;
        maxY += extraHeight;

        return new MRect(minX, minY, maxX, maxY);
    }

    private static MemoryLayer CreatePanBoundsLayer(MRect panBounds)
    {
        // This layer is only for visualizing the pan bounds. It is not needed for the limiter.
        return new MemoryLayer("PanBounds")
        {
            Features = new[] { new RectFeature(panBounds) },
            Style = new VectorStyle { Fill = null }
        };
    }
}