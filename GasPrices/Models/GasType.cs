using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasPrices.Models
{
    public class GasType(string? name)
    {
        public string? Name { get; set; } = name;

        public override string? ToString()
        {
            return Name;
        }
    }
}
