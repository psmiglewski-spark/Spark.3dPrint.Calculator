using Spark._3dPrint.Calculator.Models;

namespace Spark._3dPrint.Calculator.Services
{
    public class CostCalculationService
    {
        public decimal CalculateTotalCost(
            decimal filamentWeight,
            decimal printTime,
            Filament filament,
            PrinterSettings settings,
            IEnumerable<ExternalMaterial>? externalMaterials,
            decimal discountPercent)
        {
            decimal filamentCost = (filamentWeight / 1000m) * (filament.SpoolPrice / (filament.SpoolWeight / 1000m));
            decimal energyCost = (settings.PrinterPower / 1000m) * printTime * settings.ElectricityCost;
            decimal externalCost = externalMaterials?.Sum(x => x.TotalPrice) ?? 0m;

            decimal subtotal = filamentCost + energyCost + externalCost;
            decimal normalizedDiscount = Math.Clamp(discountPercent, 0m, 100m);
            decimal discountValue = subtotal * (normalizedDiscount / 100m);

            return subtotal - discountValue;
        }

        public decimal CalculateCostPerPiece(decimal totalCost, int quantity)
        {
            if (quantity <= 0) return 0;
            return totalCost / quantity;
        }
    }
}
