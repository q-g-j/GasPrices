using System;
using System.Collections.Generic;
using System.Text;

namespace ApiClients.Models
{
    public class Address
    {
        public Address(string? street, string? city, string? postalCode, string? country)
        {
            Street = street;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }

        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        //Debug:
        //public string Url { get; set; }

        public string GetUriData()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Street);
            stringBuilder.Append(", ");
            stringBuilder.Append(PostalCode + " ");
            stringBuilder.Append(City);
            stringBuilder.Append(", ");
            stringBuilder.Append(Country);
            return Uri.EscapeDataString(stringBuilder.ToString());
        }
    }
}
