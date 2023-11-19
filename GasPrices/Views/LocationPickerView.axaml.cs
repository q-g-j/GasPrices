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
using Avalonia.Input;
using SkiaSharp;
using CommunityToolkit.Mvvm.Input;

namespace GasPrices.Views
{
    public partial class LocationPickerView : UserControl
    {
        public LocationPickerView()
        {
        }

        public LocationPickerView(SearchResultStore searchResultStore)
        {
            InitializeComponent();

            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            MapControl.Map.Navigator.RotationLock = false;
            MapControl.UnSnapRotationDegrees = 30;
            MapControl.ReSnapRotationDegrees = 5;

            MPoint pointLonLat;
            int zoomLevel;

            if (searchResultStore.Coords != null)
            {
                pointLonLat = new MPoint(searchResultStore.Coords.Longitude, searchResultStore.Coords.Latitude);
                zoomLevel = 2;
            }
            else
            {
                pointLonLat = new MPoint(51.163361, 10.447683);
                zoomLevel = 3000;
            }
            
            var point = SphericalMercator.FromLonLat(pointLonLat);

            MapControl.Map.Home += n =>
            {
                MapControl.Map.Navigator.CenterOnAndZoomTo(point, zoomLevel, 500);
            };
        }
    }
}
