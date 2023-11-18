using ApiClients.Models;
using Mapsui;
using Mapsui.Projections;


namespace GasPrices.Extensions
{
    public static class CoordsMapsUIExtensions
    {
        public static MPoint ToMPoint(this Coords coords)
        {
            var (x, y) = SphericalMercator.FromLonLat(coords.Longitude, coords.Latitude);
            return new MPoint(x, y);
        }
    }
}
