using System;

namespace ApiClients.Models;

public class Coords
{
    public Coords(double latitude, double longitude)
    {
        Longitude = Math.Round(longitude, 6);
        Latitude = Math.Round(latitude, 6);
    }

    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public override string ToString()
    {
        return string.Format("Lat: {0} Long: {1}", Latitude, Longitude);
    }
}