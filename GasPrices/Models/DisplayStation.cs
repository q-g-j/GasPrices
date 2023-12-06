using ApiClients.Models;
using System;
using System.Text;

namespace GasPrices.Models;

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
        PostalCodeAndCity = station.PostalCode! + " " + station.City!;
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

        IsOpen = station.IsOpen!.Value ? "Ja" : "Nein";

        Price = selectedGasType.ToString() switch
        {
            "E5" => E5,
            "E10" => E10,
            "Diesel" => Diesel,
            _ => Price
        };

        if (Distance < 1)
        {
            DistanceUnit = "m";
            DistanceInUnit = Distance * 1000;
        }
        else
        {
            DistanceInUnit = Distance;
        }

        E5Thousandth = int.Parse(E5.ToString()![4].ToString());
        E10Thousandth = int.Parse(E10.ToString()![4].ToString());
        DieselThousandth = int.Parse(Diesel.ToString()![4].ToString());
        SelectedFuelThousandth = int.Parse(Price.ToString()![4].ToString());
        Price = double.Parse(Price.ToString()![..4]);
    }

    public string Name { get; set; }
    public string Brand { get; set; }
    public double Distance { get; set; }
    public double DistanceInUnit { get; set; }
    public string? DistanceUnit { get; set; } = "km";
    public string Street { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string PostalCodeAndCity { get; set; }
    public string FullAddress { get; set; }
    public double? Diesel { get; set; }
    public double? E5 { get; set; }
    public double? E10 { get; set; }
    public double? Price { get; set; }
    public int SelectedFuelThousandth { get; set; }
    public int E5Thousandth { get; set; }
    public int E10Thousandth { get; set; }
    public int DieselThousandth { get; set; }
    public string IsOpen { get; set; }

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