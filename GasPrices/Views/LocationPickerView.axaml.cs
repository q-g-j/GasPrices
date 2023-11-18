using Avalonia.Controls;
using Mapsui.Tiling;

namespace GasPrices.Views
{
    public partial class LocationPickerView : UserControl
    {
        public LocationPickerView()
        {
            InitializeComponent();

            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapControl.Map.Navigator.RotationLock = false;
            MapControl.UnSnapRotationDegrees = 30;
            MapControl.ReSnapRotationDegrees = 5;
        }
    }
}
