using ApiClients;
using Avalonia.Controls;
using GasPrices.Store;
using Mapsui.Tiling;

namespace GasPrices.Views
{
    public partial class LocationPickerView : UserControl
    {
        private readonly SearchResultStore _searchResultStore;

        public LocationPickerView(SearchResultStore searchResultStore)
        {
            InitializeComponent();
            _searchResultStore = searchResultStore;

            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapControl.Map.Navigator.RotationLock = false;
            MapControl.Map.Navigator.CenterOn(_searchResultStore.Coords!.Longitude, _searchResultStore.Coords!.Latitude);
            MapControl.UnSnapRotationDegrees = 30;
            MapControl.ReSnapRotationDegrees = 5;
        }
    }
}
