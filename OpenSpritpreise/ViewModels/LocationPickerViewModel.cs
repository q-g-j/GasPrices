using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClients.Models;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Limiting;
using Mapsui.Nts.Extensions;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Avalonia;
using Mapsui.UI.Avalonia.Extensions;
using OpenSpritpreise.Extensions;
using OpenSpritpreise.PageTransitions;
using OpenSpritpreise.Services;
using OpenSpritpreise.Store;
using SettingsHandling;

namespace OpenSpritpreise.ViewModels;

public partial class LocationPickerViewModel : ViewModelBase
{
    #region constructors

    public LocationPickerViewModel()
    {
    }

    public LocationPickerViewModel(
        NavigationService<MainNavigationStore> mainNavigationService,
        AppStateStore appStateStore,
        ISettingsReader settingsReader)
    {
        _mainNavigationService = mainNavigationService;
        _appStateStore = appStateStore;
        _settingsReader = settingsReader;

        ApplyButtonIsEnabled = _appStateStore.CoordsFromMapClient != null;

        if (OperatingSystem.IsAndroid())
        {
            BackButtonIsVisible = false;
        }

        App.GetCurrent().BackPressed += OnBackPressed;

        Initialize().FireAndForget();
    }

    #endregion constructors

    #region private fields

    private readonly NavigationService<MainNavigationStore>? _mainNavigationService;
    private readonly AppStateStore? _appStateStore;
    private readonly ISettingsReader? _settingsReader;
    private GenericCollectionLayer<List<IFeature>>? _pinLayer;
    private MPoint? _cachedPoint;
    private MPoint? _cachedMapPoint;

    #endregion private fields

    #region bindable properties

    [ObservableProperty] private bool _applyButtonIsEnabled;
    [ObservableProperty] private bool _backButtonIsVisible = true;
    [ObservableProperty] private MapControl? _mapControlObj;

    #endregion bindable properties

    #region commands

    [RelayCommand]
    private void Apply()
    {
        _mainNavigationService?.Navigate<AddressSelectionViewModel, CustomCrossFadePageTransition>();
    }

    [RelayCommand]
    private void Back()
    {
        OnBackPressed();
    }

    #endregion commands

    #region private methods

    private void OnHome(Navigator navigator)
    {
        // if there are cached coordinates, zoom in and place a pin:
        if (_cachedPoint == null) return;

        var duration = 0;

        if (!OperatingSystem.IsBrowser())
        {
            duration = 1000;

            // fix cached coords not being centered during zooming animation:
            MapControlObj!.Map.Navigator.CenterOnAndZoomTo(_cachedMapPoint!, 500);
        }

        // Center the map on the cached point
        navigator.CenterOnAndZoomTo(_cachedMapPoint!, 3, duration);

        // Place the pin
        PlacePin(_cachedMapPoint!);
    }

    private void OnTapped(object? _, TappedEventArgs e)
    {
        var tapPosition = e.GetPosition(MapControlObj).ToMapsui();
        var mapInfo = MapControlObj!.GetMapInfo(tapPosition);

        if (mapInfo?.WorldPosition == null) return;
        PlacePin(mapInfo.WorldPosition!);

        var pos = SphericalMercator.ToLonLat(mapInfo.WorldPosition!);
        var coords = new Coords(pos.Y, pos.X);
        _appStateStore!.CoordsFromMapClient = coords;
        ApplyButtonIsEnabled = true;
        e.Handled = true;
    }

    private void SetupMap()
    {
        MapControlObj = new MapControl();

        // Add OpenStreeMap Layer:
        MapControlObj.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

        // Limit viewport size to Germany:
        var germanyPanBounds = GetLimitsOfGermany();
        MapControlObj.Map.Layers.Add(CreatePanBoundsLayer(germanyPanBounds));
        MapControlObj.Map.Navigator.Limiter = new ViewportLimiterKeepWithinExtent();
        MapControlObj.Map.Navigator.OverridePanBounds = germanyPanBounds;

        // Add pins Layer:
        _pinLayer = new GenericCollectionLayer<List<IFeature>>
        {
            Style = SymbolStyles.CreatePinStyle()
        };
        MapControlObj.Map.Layers.Add(_pinLayer);
        MapControlObj.Map.Layers[0].IsMapInfoLayer = false;

        // Convert cached world coordinates to map coordinates:
        var pointLonLat = _cachedPoint ?? new MPoint(10.447683, 51.163361);
        _cachedMapPoint = SphericalMercator.FromLonLat(pointLonLat);

        // Disable rotation:        
        MapControlObj.Map.Navigator.RotationLock = true;

        MapControlObj.Map.Home = OnHome;
        MapControlObj.Tapped += OnTapped;
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

    private void OnBackPressed()
    {
        _appStateStore!.CoordsFromMapClient = null;
        _mainNavigationService!.Navigate<AddressSelectionViewModel, CustomCrossFadePageTransition>();
    }

    #endregion private methods

    #region public overrides

    public override void Dispose()
    {
        MapControlObj!.Tapped -= OnTapped;
        App.GetCurrent().BackPressed -= OnBackPressed;
    }

    #endregion public overrides
}