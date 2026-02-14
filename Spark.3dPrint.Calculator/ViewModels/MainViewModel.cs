using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spark._3dPrint.Calculator.Data;
using Spark._3dPrint.Calculator.Models;
using Spark._3dPrint.Calculator.Services;
using System.Collections.ObjectModel;

namespace Spark._3dPrint.Calculator.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;
        private readonly CostCalculationService _calculationService;

        [ObservableProperty]
        private ObservableCollection<Filament> filaments = new();

        [ObservableProperty]
        private ObservableCollection<ExternalMaterial> externalMaterials = new();

        [ObservableProperty]
        private Filament? selectedFilament;

        [ObservableProperty]
        private decimal filamentWeight;

        [ObservableProperty]
        private decimal printTimeHours;

        [ObservableProperty]
        private int quantity = 1;

        [ObservableProperty]
        private decimal discountPercent;

        [ObservableProperty]
        private string projectName = string.Empty;

        [ObservableProperty]
        private bool saveToArchive;

        [ObservableProperty]
        private decimal totalCost;

        [ObservableProperty]
        private decimal costPerPiece;

        [ObservableProperty]
        private decimal externalMaterialsCost;

        public MainViewModel(AppDbContext dbContext, CostCalculationService calculationService)
        {
            _dbContext = dbContext;
            _calculationService = calculationService;
            LoadFilaments();
        }

        private void LoadFilaments()
        {
            var filamentList = _dbContext.Filaments
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Manufacturer)
                .ToList();

            Filaments = new ObservableCollection<Filament>(filamentList);
            SelectedFilament = Filaments.FirstOrDefault();
        }

        [RelayCommand]
        private async Task AddExternalMaterial()
        {
            string name = await Shell.Current.DisplayPromptAsync("Material zewnetrzny", "Nazwa (np. magnes, grawer):");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string? unitPriceText = await Shell.Current.DisplayPromptAsync("Material zewnetrzny", "Cena jednostkowa (zl):", keyboard: Keyboard.Numeric);
            if (!decimal.TryParse(unitPriceText, out decimal unitPrice) || unitPrice < 0)
            {
                await Shell.Current.DisplayAlert("Blad", "Niepoprawna cena.", "OK");
                return;
            }

            string? quantityText = await Shell.Current.DisplayPromptAsync("Material zewnetrzny", "Ilosc:", initialValue: "1", keyboard: Keyboard.Numeric);
            if (!int.TryParse(quantityText, out int externalQuantity) || externalQuantity <= 0)
            {
                await Shell.Current.DisplayAlert("Blad", "Niepoprawna ilosc.", "OK");
                return;
            }

            ExternalMaterials.Add(new ExternalMaterial
            {
                Name = name,
                UnitPrice = unitPrice,
                Quantity = externalQuantity
            });

            ExternalMaterialsCost = ExternalMaterials.Sum(x => x.TotalPrice);
        }

        [RelayCommand]
        private void RemoveExternalMaterial(ExternalMaterial material)
        {
            if (ExternalMaterials.Contains(material))
            {
                ExternalMaterials.Remove(material);
                ExternalMaterialsCost = ExternalMaterials.Sum(x => x.TotalPrice);
            }
        }

        [RelayCommand]
        private async Task Calculate()
        {
            if (SelectedFilament == null) return;

            var settings = _dbContext.PrinterSettings.FirstOrDefault();
            if (settings == null) return;

            ExternalMaterialsCost = ExternalMaterials.Sum(x => x.TotalPrice);

            TotalCost = _calculationService.CalculateTotalCost(
                FilamentWeight,
                PrintTimeHours,
                SelectedFilament,
                settings,
                ExternalMaterials,
                DiscountPercent
            );

            CostPerPiece = _calculationService.CalculateCostPerPiece(TotalCost, Quantity);

            if (SaveToArchive)
            {
                await SaveCurrentQuoteToArchive();
            }
        }

        private async Task SaveCurrentQuoteToArchive()
        {
            var entry = new QuoteArchiveEntry
            {
                ProjectName = string.IsNullOrWhiteSpace(ProjectName) ? "Bez nazwy" : ProjectName,
                CreatedAt = DateTime.UtcNow,
                FilamentType = SelectedFilament?.Type ?? string.Empty,
                FilamentManufacturer = SelectedFilament?.Manufacturer ?? string.Empty,
                FilamentWeight = FilamentWeight,
                PrintTimeHours = PrintTimeHours,
                Quantity = Quantity,
                DiscountPercent = DiscountPercent,
                ExternalMaterialsCost = ExternalMaterialsCost,
                TotalCost = TotalCost,
                CostPerPiece = CostPerPiece
            };

            _dbContext.QuoteArchiveEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            await Shell.Current.DisplayAlert("Archiwum", "Wycena zostala zapisana w archiwum.", "OK");
        }

        [RelayCommand]
        private async Task NavigateToSettings()
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }

        [RelayCommand]
        private async Task NavigateToArchive()
        {
            await Shell.Current.GoToAsync("ArchivePage");
        }
    }
}
