using Microsoft.EntityFrameworkCore;
using Spark._3dPrint.Calculator.Models;

namespace Spark._3dPrint.Calculator.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Filament> Filaments { get; set; }
        public DbSet<PrinterSettings> PrinterSettings { get; set; }
        public DbSet<QuoteArchiveEntry> QuoteArchiveEntries { get; set; }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "printing.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Filament>().HasData(
                new Filament { Id = 1, Type = "PLA", Manufacturer = "Przykladowy", SpoolPrice = 80.00m, SpoolWeight = 1000 },
                new Filament { Id = 2, Type = "PETG", Manufacturer = "Przykladowy", SpoolPrice = 95.00m, SpoolWeight = 1000 },
                new Filament { Id = 3, Type = "ABS", Manufacturer = "Przykladowy", SpoolPrice = 90.00m, SpoolWeight = 1000 }
            );

            modelBuilder.Entity<PrinterSettings>().HasData(
                new PrinterSettings { Id = 1, ElectricityCost = 0.80m, PrinterPower = 250 }
            );
        }
    }
}
