using System;
using System.Collections.Generic;
using System.Text;

namespace ApiClients.Models
{
    public class Coords
    {
        public Coords(double latitude, double longitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public override string ToString()
        {
            return string.Format("Long: {0}, Lat: {1}", Longitude, Latitude);
        }
    }
}
