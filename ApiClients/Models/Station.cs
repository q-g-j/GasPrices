using System;
using System.Collections.Generic;
using System.Text;

namespace ApiClients.Models
{
    public class Station
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public int? PostalCode { get; set; }
        public string? City { get; set; }
        public double? Distance { get; set; }
        public double? E5 { get; set; }
        public double? E10 { get; set; }
        public double? Diesel { get; set; }
        public bool? IsOpen { get; set; }
    }
}
