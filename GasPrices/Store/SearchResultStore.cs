using ApiClients.Models;
using GasPrices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasPrices.Store
{
    public class SearchResultStore
    {
        public List<Station>? Stations { get; set; }
        public GasType? SelectedGasType { get; set; }
        public Address? Address { get; set; }
        public Coords? Coords { get; set; }
        public int? Distance { get; set; }
    }
}
