using System;
using System.Collections.Generic;
using System.Text;

namespace ApiClients.Models
{
    public class Address
    {
        public Address(string? street, string? city, string? postalCode)
        {
            Street = street;
            City = city;
            PostalCode = postalCode;
        }

        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

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
}
