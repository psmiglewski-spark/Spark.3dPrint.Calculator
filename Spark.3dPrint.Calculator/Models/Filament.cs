namespace Spark._3dPrint.Calculator.Models
{
    public class Filament
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public decimal SpoolPrice { get; set; } // Cena za 1kg
        public int SpoolWeight { get; set; } = 1000; // Domyœlnie 1kg w gramach
    }
}