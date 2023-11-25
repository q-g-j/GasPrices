using ApiClients.Models;
using System;
using System.Text;

namespace GasPrices.Models
{
    public class DisplayStation
    {
        public DisplayStation(Station station, GasType selectedGasType)
        {
            Name = station.Name!;
            Brand = string.IsNullOrEmpty(station.Brand!) ? station.Name! : station.Brand!;
            Distance = Math.Round(station.Distance!.Value, 2);
            Street = station.Street!;
            if (!string.IsNullOrEmpty(station.Street) && !string.IsNullOrEmpty(station.HouseNumber))
            {
                Street += " " + station.HouseNumber;
            }
            PostalCode = station.PostalCode!.ToString()!;
            City = station.City!;

            FullAddress = Street + ", " + PostalCode + " " + City;
            
            if (station.E5 != 0)
            {
                E5 = station.E5;
            }
            if (station.E10 != 0)
            {
                E10 = station.E10;
            }
            if (station.Diesel != 0)
            {
                Diesel = station.Diesel;
            }
            IsOpen = station.IsOpen;

            switch (selectedGasType.ToString())
            {
                case "E5":
                    Price = E5;
                    break;
                case "E10":
                    Price = E10;
                    break;
                case "Diesel":
                    Price = Diesel;
                    break;
            }

            if (Distance < 1)
            {
                DistanceUnit = "m";
                Distance *= 1000;
            }
            
            PriceThousandth = int.Parse(Price.ToString()![4].ToString());
            Price = double.Parse(Price.ToString()![..4]);
        }

        public string Name { get; set; }
        public string Brand { get; set; }
        public double Distance { get; set; }
        public string? DistanceUnit { get; set; } = "km";
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string FullAddress { get; set; }
        public double? Diesel { get; set; }
        public double? E5 { get; set; }
        public double? E10 { get; set; }
        public double? Price { get; set; }
        public int PriceThousandth { get; set; }
        public bool? IsOpen { get; set; }

        public string GetUriData()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Name + ", ");
            stringBuilder.Append(Street + ", ");
            stringBuilder.Append(PostalCode + " ");
            stringBuilder.Append(City);
            return Uri.EscapeDataString(stringBuilder.ToString());
        }
    }
}
