namespace Spark._3dPrint.Calculator.Models
{
    public class PrinterSettings
    {
        public int Id { get; set; }
        public decimal ElectricityCost { get; set; } 
        public decimal PrinterPower { get; set; } // Moc drukarki w watach
    }
}