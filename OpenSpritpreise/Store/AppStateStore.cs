using ApiClients.Models;
using OpenSpritpreise.Models;
using System.Collections.Generic;

namespace OpenSpritpreise.Store;

public class AppStateStore
{
    public List<Station>? Stations { get; set; }
    public GasType? SelectedGasType { get; set; }
    public Address? Address { get; set; }
    public Coords? CoordsFromMapClient { get; set; }
    public int? Distance { get; set; }
    public DisplayStation? SelectedStation { get; set; }
    public int SelectedStationIndex { get; set; }
}