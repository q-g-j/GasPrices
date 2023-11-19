using Avalonia.Controls;
using GasPrices.Store;
using Mapsui.Tiling;
using ApiClients.Models;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using GasPrices.Services;
using Mapsui.Layers;
using Mapsui.Styles;
using System.Collections.Generic;
using Mapsui.Nts;
using Mapsui.UI.Avalonia;
using Avalonia.Input;
using System;
using BruTile.Wms;
using Mapsui.UI.Avalonia.Extensions;
using NetTopologySuite.Geometries;
using GasPrices.ViewModels;

namespace GasPrices.Views
{
    public partial class LocationPickerView : UserControl
    {
        public LocationPickerView()
        {
        }

        public LocationPickerView(SearchResultStore searchResultStore)
        {
            _searchResultStore = searchResultStore;

            InitializeComponent();

            SetupMap();
        }

        private readonly SearchResultStore? _searchResultStore;
        private GenericCollectionLayer<List<IFeature>>? _pinLayer;

        private void SetupMap()
        {
            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            MapControl.Map.Navigator.RotationLock = false;
            MapControl.UnSnapRotationDegrees = 30;
            MapControl.ReSnapRotationDegrees = 5;

            MPoint pointLonLat;
            int zoomLevel;

            if (_searchResultStore!.Coords != null)
            {
                pointLonLat = new MPoint(_searchResultStore!.Coords.Longitude, _searchResultStore!.Coords.Latitude);
                zoomLevel = 2;
            }
            else
            {
                pointLonLat = new MPoint(51.163361, 10.447683);
                zoomLevel = 3000;
            }

            _pinLayer = new GenericCollectionLayer<List<IFeature>>
            {
                Style = SymbolStyles.CreatePinStyle()
            };
            MapControl.Map.Layers.Add(_pinLayer);

            var point = SphericalMercator.FromLonLat(pointLonLat);
            MapControl.Map.Layers[0].IsMapInfoLayer = false;

            MapControl.Map.Home += n =>
            {
                MapControl.Map.Navigator.CenterOnAndZoomTo(point, zoomLevel, 500);
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
                    _searchResultStore!.Coords = coords;
                    ((LocationPickerViewModel)DataContext!).ApplyButtonIsEnabled = true;
                }
            };
        }
    }
}
