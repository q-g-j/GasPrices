using Avalonia.Controls;

namespace GasPrices.Views
{
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();

            dgStations.Loaded += (o, e) =>
            {
                dgStations.Columns[2].Sort(System.ComponentModel.ListSortDirection.Ascending);
                dgStations.SelectedItem = null;
            };
        }
    }
}
