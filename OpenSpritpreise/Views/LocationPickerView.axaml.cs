using System;
using Avalonia.Controls;
using OpenSpritpreise.Store;
using Mapsui.Tiling;
using ApiClients.Models;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Layers;
using Mapsui.Styles;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenSpritpreise.Extensions;
using Mapsui.Nts;
using Mapsui.UI.Avalonia.Extensions;
using OpenSpritpreise.ViewModels;
using Mapsui.Limiting;
using SettingsHandling;

namespace OpenSpritpreise.Views;

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
        // Add OpenStreeMap Layer:
        MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        
        // Limit viewport size to Germany:
        var germanyPanBounds = GetLimitsOfGermany();
        MapControl.Map.Layers.Add(CreatePanBoundsLayer(germanyPanBounds));
        MapControl.Map.Navigator.Limiter = new ViewportLimiterKeepWithinExtent();
        MapControl.Map.Navigator.OverridePanBounds = germanyPanBounds;

        // Add pins Layer:
        _pinLayer = new GenericCollectionLayer<List<IFeature>>
        {
            Style = SymbolStyles.CreatePinStyle()
        };
        MapControl.Map.Layers.Add(_pinLayer);
        MapControl.Map.Layers[0].IsMapInfoLayer = false;

        // Convert cached world coordinates to map coordinates:
        var pointLonLat = _cachedPoint ?? new MPoint(10.447683, 51.163361);
        _cachedMapPoint = SphericalMercator.FromLonLat(pointLonLat);
        
        // Disable rotation:        
        MapControl.Map.Navigator.RotationLock = true;

        // Setup Home-Event (fired, when map is loaded):
        MapControl.Map.Home += n =>
        {
            // if there are cached coordinates, zoom in and place a pin:
            if (_cachedPoint == null) return;

            var duration = 0;

            if (!OperatingSystem.IsBrowser())
            {
                duration = 1000;
                
                // fix cached coords not being centered during zooming animation:
                MapControl.Map.Navigator.CenterOnAndZoomTo(_cachedMapPoint, 500);
            }

            // Center the map on the cached point
            n.CenterOnAndZoomTo(_cachedMapPoint, 3, duration);

            // Place the pin
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
    
    // Try to get existing coordinates from settings, then from the app state store:
    private async Task ProcessSettingsAndState()
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
    }

    private async Task Initialize()
    {
        await ProcessSettingsAndState();
        
        SetupMap();
    }
    
    private static MRect GetLimitsOfGermany()
    {
        var (minX, minY) = SphericalMercator.FromLonLat(5.866240, 47.270111);
        var (maxX, maxY) = SphericalMercator.FromLonLat(15.042050, 54.983104);

        // Increase the height to both sides:
        const double extraHeight = 100000.0;
        minY -= extraHeight;
        maxY += extraHeight;

        return new MRect(minX, minY, maxX, maxY);
    }

    private static MemoryLayer CreatePanBoundsLayer(MRect panBounds)
    {
        return new MemoryLayer("PanBounds")
        {
            Features = new[] { new RectFeature(panBounds) },
            Style = new VectorStyle { Fill = null }
        };
    }
}