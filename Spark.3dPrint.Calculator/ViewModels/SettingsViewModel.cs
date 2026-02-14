using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spark._3dPrint.Calculator.Data;
using Spark._3dPrint.Calculator.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

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
            var filamentList = _dbContext.Filaments
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Manufacturer)
                .ToList();
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
        private async Task ImportFilamentsFromPackage()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("default_filaments.json");
            using var reader = new StreamReader(stream);
            string json = await reader.ReadToEndAsync();

            var imported = JsonSerializer.Deserialize<List<FilamentImportItem>>(json) ?? new List<FilamentImportItem>();
            int added = 0;

            foreach (var item in imported)
            {
                if (string.IsNullOrWhiteSpace(item.Type) || string.IsNullOrWhiteSpace(item.Manufacturer))
                {
                    continue;
                }

                bool exists = _dbContext.Filaments.Any(x => x.Type == item.Type && x.Manufacturer == item.Manufacturer);
                if (exists)
                {
                    continue;
                }

                _dbContext.Filaments.Add(new Filament
                {
                    Type = item.Type,
                    Manufacturer = item.Manufacturer,
                    SpoolPrice = item.SpoolPrice,
                    SpoolWeight = item.SpoolWeight <= 0 ? 1000 : item.SpoolWeight
                });

                added++;
            }

            await _dbContext.SaveChangesAsync();
            LoadData();

            await Shell.Current.DisplayAlert("Import filamentow", $"Dodano {added} nowych filamentow.", "OK");
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
            LoadData();

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
                LoadData();
            }
        }

        private class FilamentImportItem
        {
            public string Type { get; set; } = string.Empty;
            public string Manufacturer { get; set; } = string.Empty;
            public decimal SpoolPrice { get; set; }
            public int SpoolWeight { get; set; } = 1000;
        }
    }
}
