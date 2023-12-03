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

        MapControl.Map.Navigator.RotationLock = false;
        MapControl.UnSnapRotationDegrees = 30;
        MapControl.ReSnapRotationDegrees = 5;

        MPoint pointLonLat;

        if (_cachedPoint != null)
        {
            pointLonLat = _cachedPoint;
        }
        else
        {
            pointLonLat = new MPoint(10.447683, 51.163361);
        }

        _pinLayer = new GenericCollectionLayer<List<IFeature>>
        {
            Style = SymbolStyles.CreatePinStyle()
        };
        MapControl.Map.Layers.Add(_pinLayer);
        MapControl.Map.Layers[0].IsMapInfoLayer = false;

        _cachedMapPoint = SphericalMercator.FromLonLat(pointLonLat);

        MapControl.Map.Home += _ =>
        {
            var zoomLevel = 2;
            var duration = 1000;
            if (_cachedPoint == null)
            {
                zoomLevel = 3000;
                duration = 0;
            }
            else
            {
                MapControl.Map.Navigator.CenterOnAndZoomTo(_cachedMapPoint!, 3000, 0);
                PlacePin(_cachedMapPoint);
            }

            MapControl.Map.Navigator.CenterOnAndZoomTo(_cachedMapPoint!, zoomLevel, duration);
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

        SetupMap();}
}