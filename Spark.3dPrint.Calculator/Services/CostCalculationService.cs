using Spark._3dPrint.Calculator.Models;

namespace Spark._3dPrint.Calculator.Services
{
    public class CostCalculationService
    {
        public decimal CalculateTotalCost(
            decimal filamentWeight, 
            decimal printTime, 
            Filament filament, 
            PrinterSettings settings)
        {
            // Koszt filamentu: (waga w gramach / 1000) * (cena szpuli / waga szpuli w kg)
            decimal filamentCost = (filamentWeight / 1000m) * (filament.SpoolPrice / (filament.SpoolWeight / 1000m));

            // Koszt energii: (moc w W / 1000) * czas w h * koszt za kWh
            decimal energyCost = (settings.PrinterPower / 1000m) * printTime * settings.ElectricityCost;

            return filamentCost + energyCost;
        }

        public decimal CalculateCostPerPiece(decimal totalCost, int quantity)
        {
            if (quantity <= 0) return 0;
            return totalCost / quantity;
        }
    }
}