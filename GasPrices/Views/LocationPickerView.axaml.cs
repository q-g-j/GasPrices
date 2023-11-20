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
using Mapsui.Nts;
using Mapsui.UI.Avalonia;
using Mapsui.UI.Avalonia.Extensions;
using GasPrices.ViewModels;
using SettingsFile.SettingsFile;
using System.Threading.Tasks;
using GasPrices.Utilities;

namespace GasPrices.Views
{
    public partial class LocationPickerView : UserControl
    {
        public LocationPickerView()
        {
        }

        public LocationPickerView(AppStateStore appStateStore, SettingsFileReader settingsFileReader)
        {
            _appStateStore = appStateStore;
            _settingsFileReader = settingsFileReader;
            InitializeComponent();

            var settings = _settingsFileReader.Read();
            if (settings?.LastKnownLatitude != null && settings?.LastKnownLongitude != null)
            {
                _cachedPoint = new MPoint(settings.LastKnownLongitude.Value, settings.LastKnownLatitude.Value);
            }

            SetupMap();
        }

        private readonly AppStateStore? _appStateStore;
        private readonly SettingsFileReader? _settingsFileReader;
        private GenericCollectionLayer<List<IFeature>>? _pinLayer;
        private readonly MPoint? _cachedPoint;
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

            _cachedMapPoint = SphericalMercator.FromLonLat(pointLonLat);
            MapControl.Map.Layers[0].IsMapInfoLayer = false;

            MapControl.Map.Home += n =>
            {
                int zoomLevel = _cachedPoint == null ? 3000 : 2;
                MapControl.Map.Navigator.CenterOnAndZoomTo(_cachedMapPoint!, zoomLevel, 500);
            };

            MapControl.Tapped += (s, e) =>
            {
                e.Handled = true;
                var tapPosition = e.GetPosition(MapControl).ToMapsui();
                var mapInfo = MapControl.GetMapInfo(tapPosition);

                if (mapInfo != null && mapInfo.WorldPosition != null)
                {
                    var geographicalCoordinates = SphericalMercator.ToLonLat(mapInfo.WorldPosition!);

                    double latitude = geographicalCoordinates.Y;
                    double longitude = geographicalCoordinates.X;

                    _pinLayer?.Features.Clear();
                    _pinLayer?.Features.Add(new GeometryFeature
                    {
                        Geometry = mapInfo.WorldPosition!.ToPoint()
                    });

                    _pinLayer?.DataHasChanged();

                    var pos = SphericalMercator.ToLonLat(mapInfo.WorldPosition!);
                    var coords = new Coords(pos.Y, pos.X);
                    _appStateStore!.Coords = coords;
                    ((LocationPickerViewModel)DataContext!).ApplyButtonIsEnabled = true;
                }
            };
        }
    }
}
