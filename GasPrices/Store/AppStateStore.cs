using ApiClients.Models;
using GasPrices.Models;
using System.Collections.Generic;

namespace GasPrices.Store
{
    public class AppStateStore
    {
        public List<Station>? Stations { get; set; }
        public GasType? SelectedGasType { get; set; }
        public Address? Address { get; set; }
        public Coords? CoordsFromMapClient { get; set; }
        public int? Distance { get; set; }
        public DisplayStation? SelectedStation { get; set; }
        public int SelectedStationIndex { get; set; }
        public bool IsFromStationDetailsView  { get; set; }
    }
}
