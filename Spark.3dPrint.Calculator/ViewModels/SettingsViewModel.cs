using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spark._3dPrint.Calculator.Data;
using Spark._3dPrint.Calculator.Models;
using System.Collections.ObjectModel;

namespace Spark._3dPrint.Calculator.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<Filament> filaments = new();

        [ObservableProperty]
        private decimal electricityCost;

        [ObservableProperty]
        private decimal printerPower;

        public SettingsViewModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            LoadData();
        }

        private void LoadData()
        {
            var filamentList = _dbContext.Filaments.ToList();
            Filaments = new ObservableCollection<Filament>(filamentList);

            var settings = _dbContext.PrinterSettings.FirstOrDefault();
            if (settings != null)
            {
                ElectricityCost = settings.ElectricityCost;
                PrinterPower = settings.PrinterPower;
            }
        }

        [RelayCommand]
        private async Task SavePrinterSettings()
        {
            var settings = _dbContext.PrinterSettings.FirstOrDefault();
            if (settings != null)
            {
                settings.ElectricityCost = ElectricityCost;
                settings.PrinterPower = PrinterPower;
                await _dbContext.SaveChangesAsync();
                await Shell.Current.DisplayAlert("Sukces", "Ustawienia zapisane", "OK");
            }
        }

        [RelayCommand]
        private async Task AddFilament()
        {
            string type = await Shell.Current.DisplayPromptAsync("Nowy filament", "Rodzaj filamentu (np. PLA):");
            if (string.IsNullOrWhiteSpace(type)) return;

            string manufacturer = await Shell.Current.DisplayPromptAsync("Nowy filament", "Producent:");
            if (string.IsNullOrWhiteSpace(manufacturer)) return;

            string priceStr = await Shell.Current.DisplayPromptAsync("Nowy filament", "Cena szpuli (zl):", keyboard: Keyboard.Numeric);
            if (!decimal.TryParse(priceStr, out decimal price)) return;

            var filament = new Filament
            {
                Type = type,
                Manufacturer = manufacturer,
                SpoolPrice = price,
                SpoolWeight = 1000
            };

            _dbContext.Filaments.Add(filament);
            await _dbContext.SaveChangesAsync();
            Filaments.Add(filament);

            await Shell.Current.DisplayAlert("Sukces", "Filament dodany", "OK");
        }

        [RelayCommand]
        private async Task DeleteFilament(Filament filament)
        {
            bool confirm = await Shell.Current.DisplayAlert("Potwierdzenie", 
                $"Czy na pewno usunac {filament.Type}?", "Tak", "Nie");
            
            if (confirm)
            {
                _dbContext.Filaments.Remove(filament);
                await _dbContext.SaveChangesAsync();
                Filaments.Remove(filament);
            }
        }
    }
}