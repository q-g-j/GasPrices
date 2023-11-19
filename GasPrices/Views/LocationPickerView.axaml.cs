using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using GasPrices.Store;
using Mapsui.Tiling;
using Mapsui.Utilities;
using Mapsui.UI.Objects;
using GasPrices.Models;
using GasPrices.Extensions;
using ApiClients.Models;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using System;
using GasPrices.ViewModels;
using ApiClients;
using System.Threading.Tasks;
using GasPrices.Utilities;

namespace GasPrices.Views
{
    public partial class LocationPickerView : UserControl
    {
        private readonly SearchResultStore? _searchResultStore;
        private readonly IMapClient? _mapClient;

        public LocationPickerView()
        {
        }

        public LocationPickerView(SearchResultStore searchResultStore)
        {
            InitializeComponent();
            _searchResultStore = searchResultStore;

            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            MapControl.Map.Navigator.RotationLock = false;
            MapControl.UnSnapRotationDegrees = 30;
            MapControl.ReSnapRotationDegrees = 5;

            MPoint point;
            int zoomLevel;

            if (searchResultStore.Coords != null)
            {
                point = _searchResultStore.Coords!.ToMPoint();
                zoomLevel = 2;
            }
            else
            {
                point = new Coords(51.163361, 10.447683).ToMPoint();
                zoomLevel = 3000;
            }

            MapControl.Map.Home += n =>
            {
                MapControl.Map.Navigator.CenterOnAndZoomTo(point, zoomLevel, 500);
            };

            MapControl.Map.Info += (s, a) =>
            {
                var pos = SphericalMercator.ToLonLat(a.MapInfo?.WorldPosition!);
                var coords = new Coords(pos.Y, pos.X);
                _searchResultStore.Coords = coords;
                ((LocationPickerViewModel)DataContext!)?.BackCommand();
            };
        }
    }
}
