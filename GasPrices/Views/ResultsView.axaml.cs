using Avalonia.Controls;
using GasPrices.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace GasPrices.Views
{
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();

            //    dgStations.Loaded += (o, e) =>
            //    {
            //        dgStations.Columns[2].Sort(System.ComponentModel.ListSortDirection.Ascending);
            //    };
        }
    }
}
