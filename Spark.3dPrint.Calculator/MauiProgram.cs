using Microsoft.Extensions.Logging;
using Spark._3dPrint.Calculator.Converters;
using Spark._3dPrint.Calculator.Data;
using Spark._3dPrint.Calculator.Services;
using Spark._3dPrint.Calculator.ViewModels;
using Spark._3dPrint.Calculator.Views;

namespace Spark._3dPrint.Calculator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Rejestracja serwisow
            builder.Services.AddSingleton<AppDbContext>();
            builder.Services.AddSingleton<CostCalculationService>();
            builder.Services.AddHttpClient<OnlineFilamentCatalogService>();
            builder.Services.AddSingleton<IsNotZeroConverter>();

            // Rejestracja ViewModels
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<ArchiveViewModel>();

            // Rejestracja Views
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ArchivePage>();

#if DEBUG
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
#endif

            return builder.Build();
        }
    }
}
