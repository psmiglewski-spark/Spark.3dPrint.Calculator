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
        private Filament? selectedFilament;

        [ObservableProperty]
        private decimal filamentWeight;

        [ObservableProperty]
        private decimal printTimeHours;

        [ObservableProperty]
        private int quantity = 1;

        [ObservableProperty]
        private decimal totalCost;

        [ObservableProperty]
        private decimal costPerPiece;

        public MainViewModel(AppDbContext dbContext, CostCalculationService calculationService)
        {
            _dbContext = dbContext;
            _calculationService = calculationService;
            LoadFilaments();
        }

        private void LoadFilaments()
        {
            var filamentList = _dbContext.Filaments.ToList();
            Filaments = new ObservableCollection<Filament>(filamentList);
            SelectedFilament = Filaments.FirstOrDefault();
        }

        [RelayCommand]
        private void Calculate()
        {
            if (SelectedFilament == null) return;

            var settings = _dbContext.PrinterSettings.FirstOrDefault();
            if (settings == null) return;

            TotalCost = _calculationService.CalculateTotalCost(
                FilamentWeight,
                PrintTimeHours,
                SelectedFilament,
                settings
            );

            CostPerPiece = _calculationService.CalculateCostPerPiece(TotalCost, Quantity);
        }

        [RelayCommand]
        private async Task NavigateToSettings()
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }
    }
}