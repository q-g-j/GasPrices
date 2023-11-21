using ApiClients.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasPrices.Models
{
    public class DisplayStation
    {
        public DisplayStation(Station station, GasType selectedGasType)
        {
            Name = station.Name!;
            Brand = string.IsNullOrEmpty(station!.Brand!) ? station!.Name! : station!.Brand!;
            Distance = Math.Round(station.Distance!.Value, 2);
            if (!string.IsNullOrEmpty(station.Street))
            {
                Street = station.Street + " ";
            }
            Street += station.HouseNumber;
            PostalCode = station.PostalCode!.ToString()!;
            City = station.City!.ToString();
            if (station.Diesel != 0)
            {
                Diesel = station.Diesel.ToString() + " €";
            }
            if (station.E5 != 0)
            {
                E5 = station.E5.ToString() + " €";
            }
            if (station.E10 != 0)
            {
                E10 = station.E10.ToString() + " €";
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
                default:
                    break;
            }
        }

        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public double Distance { get; set; }
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Diesel { get; set; } = string.Empty;
        public string E5 { get; set; } = string.Empty;
        public string E10 { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public bool? IsOpen { get; set; } = false;

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
