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
