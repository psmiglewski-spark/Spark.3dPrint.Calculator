namespace Spark._3dPrint.Calculator.Models
{
    public class Filament
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public decimal SpoolPrice { get; set; }
        public int SpoolWeight { get; set; } = 1000;

        public string DisplayName => $"{Type} - {Manufacturer} ({SpoolPrice:F2} zl)";
    }
}
