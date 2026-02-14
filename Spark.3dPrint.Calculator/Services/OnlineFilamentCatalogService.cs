using System.Globalization;
using System.Text.RegularExpressions;

namespace Spark._3dPrint.Calculator.Services
{
    public class OnlineFilamentCatalogService
    {
        private static readonly string[] FilamentTypes = ["PLA", "PETG", "ABS", "ASA", "TPU"];
        private static readonly string[] Manufacturers = ["Devil Design", "Fiberlogy", "Sunlu", "Prusa", "Noctuo", "eSUN", "Bambu Lab", "Spectrum", "Nebula", "Polymaker"];
        private static readonly Regex HtmlTagRegex = new("<.*?>", RegexOptions.Compiled);
        private static readonly Regex PriceRegex = new(@"(\d{2,4}(?:[\.,]\d{1,2})?)\s?(?:zł|PLN)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly HttpClient _httpClient;

        public OnlineFilamentCatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Spark3dPrintCalculator/1.0");
            }
        }

        public async Task<IReadOnlyList<OnlineFilamentAverage>> DownloadAveragedCatalogAsync(CancellationToken cancellationToken = default)
        {
            var offers = new List<OnlineFilamentOffer>();

            foreach (var type in FilamentTypes)
            {
                var foundForType = await SearchOffersForTypeAsync(type, cancellationToken);
                offers.AddRange(foundForType);
            }

            var averaged = offers
                .Where(x => x.PricePerSpoolPln > 0)
                .GroupBy(x => new
                {
                    x.Type,
                    x.Manufacturer,
                    x.SpoolWeight
                })
                .Select(group => new OnlineFilamentAverage
                {
                    Type = group.Key.Type,
                    Manufacturer = group.Key.Manufacturer,
                    SpoolWeight = group.Key.SpoolWeight,
                    AverageSpoolPrice = decimal.Round(group.Average(x => x.PricePerSpoolPln), 2),
                    OfferCount = group.Count()
                })
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Manufacturer)
                .ToList();

            return averaged;
        }

        private async Task<IEnumerable<OnlineFilamentOffer>> SearchOffersForTypeAsync(string type, CancellationToken cancellationToken)
        {
            var query = Uri.EscapeDataString($"filament {type} 1kg cena sklep");
            var url = $"https://duckduckgo.com/html/?q={query}";

            var html = await _httpClient.GetStringAsync(url, cancellationToken);
            if (string.IsNullOrWhiteSpace(html))
            {
                return [];
            }

            var rows = html.Split("result__body", StringSplitOptions.RemoveEmptyEntries);
            var offers = new List<OnlineFilamentOffer>();

            foreach (var row in rows)
            {
                var text = NormalizeText(HtmlTagRegex.Replace(row, " "));
                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                var price = ParsePrice(text);
                if (price <= 0)
                {
                    continue;
                }

                var manufacturer = DetectManufacturer(text);
                offers.Add(new OnlineFilamentOffer
                {
                    Type = type,
                    Manufacturer = manufacturer,
                    PricePerSpoolPln = price,
                    SpoolWeight = 1000
                });
            }

            return offers;
        }

        private static decimal ParsePrice(string value)
        {
            var match = PriceRegex.Match(value);
            if (!match.Success)
            {
                return 0;
            }

            var normalized = match.Groups[1].Value.Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var price)
                ? price
                : 0;
        }

        private static string DetectManufacturer(string text)
        {
            foreach (var manufacturer in Manufacturers)
            {
                if (text.Contains(manufacturer, StringComparison.OrdinalIgnoreCase))
                {
                    return manufacturer;
                }
            }

            return "Różni producenci";
        }

        private static string NormalizeText(string value)
        {
            return Regex.Replace(value, "\\s+", " ").Trim();
        }

        private sealed class OnlineFilamentOffer
        {
            public string Type { get; set; } = string.Empty;
            public string Manufacturer { get; set; } = string.Empty;
            public decimal PricePerSpoolPln { get; set; }
            public int SpoolWeight { get; set; }
        }
    }

    public sealed class OnlineFilamentAverage
    {
        public string Type { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public decimal AverageSpoolPrice { get; set; }
        public int SpoolWeight { get; set; }
        public int OfferCount { get; set; }
    }
}
