namespace SettingsFile.Models
{
    public class Settings
    {
        public string? TankerkönigApiKey { get; set; }
        public string? LastKnownStreet { get; set; }
        public string? LastKnownPostalCode { get; set; }
        public string? LastKnownCity { get; set; }
        public double? LastKnownLatitude { get; set; }
        public double? LastKnownLongitude { get; set; }
        public string? LastKnownGasType { get; set; } = "E5";
        public string? LastKnownRadius { get; set; } = "5";
        public string? SortBy { get; set; }
    }
}
