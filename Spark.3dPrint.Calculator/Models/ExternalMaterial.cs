namespace Spark._3dPrint.Calculator.Models
{
    public class ExternalMaterial
    {
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
