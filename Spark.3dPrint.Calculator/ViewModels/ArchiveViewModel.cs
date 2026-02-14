using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spark._3dPrint.Calculator.Data;
using Spark._3dPrint.Calculator.Models;
using System.Collections.ObjectModel;

namespace Spark._3dPrint.Calculator.ViewModels
{
    public partial class ArchiveViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;
        private List<QuoteArchiveEntry> _allEntries = new();

        [ObservableProperty]
        private ObservableCollection<QuoteArchiveEntry> entries = new();

        [ObservableProperty]
        private ObservableCollection<string> filamentFilters = new();

        [ObservableProperty]
        private string selectedFilamentFilter = "Wszystkie";

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool onlyDiscounted;

        public ArchiveViewModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            LoadEntries();
        }

        private void LoadEntries()
        {
            _allEntries = _dbContext.QuoteArchiveEntries
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var filters = _allEntries
                .Select(x => x.FilamentType)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            filters.Insert(0, "Wszystkie");
            FilamentFilters = new ObservableCollection<string>(filters);
            ApplyFilters();
        }

        partial void OnSearchTextChanged(string value) => ApplyFilters();
        partial void OnSelectedFilamentFilterChanged(string value) => ApplyFilters();
        partial void OnOnlyDiscountedChanged(bool value) => ApplyFilters();

        [RelayCommand]
        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedFilamentFilter = "Wszystkie";
            OnlyDiscounted = false;
            ApplyFilters();
        }

        [RelayCommand]
        private void Refresh() => LoadEntries();

        private void ApplyFilters()
        {
            IEnumerable<QuoteArchiveEntry> query = _allEntries;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string term = SearchText.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    x.ProjectName.ToLowerInvariant().Contains(term)
                    || x.FilamentType.ToLowerInvariant().Contains(term)
                    || x.FilamentManufacturer.ToLowerInvariant().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(SelectedFilamentFilter) && SelectedFilamentFilter != "Wszystkie")
            {
                query = query.Where(x => x.FilamentType == SelectedFilamentFilter);
            }

            if (OnlyDiscounted)
            {
                query = query.Where(x => x.DiscountPercent > 0);
            }

            Entries = new ObservableCollection<QuoteArchiveEntry>(query.OrderByDescending(x => x.CreatedAt));
        }
    }
}
