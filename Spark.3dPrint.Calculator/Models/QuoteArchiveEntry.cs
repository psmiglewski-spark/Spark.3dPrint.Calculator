namespace Spark._3dPrint.Calculator.Models
{
    public class QuoteArchiveEntry
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FilamentType { get; set; } = string.Empty;
        public string FilamentManufacturer { get; set; } = string.Empty;
        public decimal FilamentWeight { get; set; }
        public decimal PrintTimeHours { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal ExternalMaterialsCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CostPerPiece { get; set; }
    }
}
