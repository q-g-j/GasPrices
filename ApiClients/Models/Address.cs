using System;
using System.Text;

namespace ApiClients.Models;

public class Address(string? street, string? city, string? postalCode, string? country = "Deutschland")
{
    public string? Street { get; set; } = street;
    public string? City { get; set; } = city;
    public string? PostalCode { get; set; } = postalCode;
    public string? Country { get; set; } = country;

    public string GetUriData()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Street);
        stringBuilder.Append(", ");
        stringBuilder.Append(PostalCode + " ");
        stringBuilder.Append(City);
        return Uri.EscapeDataString(stringBuilder.ToString());
    }
}