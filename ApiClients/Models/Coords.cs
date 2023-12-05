using System;

namespace ApiClients.Models;

public class Coords(double latitude, double longitude)
{
    public double Longitude { get; set; } = Math.Round(longitude, 6);
    public double Latitude { get; set; } = Math.Round(latitude, 6);

    public override string ToString()
    {
        return string.Format("Lat: {0} Long: {1}", Latitude, Longitude);
    }
}