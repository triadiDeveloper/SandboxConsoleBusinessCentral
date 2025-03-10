using ConsoleBusinessCentral.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace ConsoleBusinessCentral.Persistence
{
    public class ConsoleDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // Constructor dengan DbContextOptions + IConfiguration
        // Biasanya digunakan saat DI di Program.cs
        public ConsoleDbContext(DbContextOptions<ConsoleDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        // DbSet untuk Customer
        public DbSet<Customer> Customers { get; set; }

        // Jika mau men-override konfigurasi database di sini (OnConfiguring)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Hanya jika options belum di-set (tidak di DI)
            if (!optionsBuilder.IsConfigured)
            {
                if (_configuration == null)
                {
                    // Misal fallback hardcoded
                    // Ganti dengan logika Anda jika config null
                    optionsBuilder.UseSqlServer(
                        @"Server=.\SQLEXPRESS;Database=DEV;Trusted_Connection=True;",
                        opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
                    );
                }
                else
                {
                    // Mengambil dari IConfiguration yang disimpan di field
                    optionsBuilder.UseSqlServer(
                        _configuration.GetConnectionString("DEV"),
                        opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
                    );
                }
            }
        }

        // Jika ingin Fluent API khusus, tambahkan di OnModelCreating:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                // Set 'No' sebagai primary key (sesuai pertanyaan sebelumnya)
                entity.HasKey(c => c.No);

                // Tentukan precision/scale decimal
                entity.Property(e => e.Credit_Limit_LCY).HasPrecision(18, 2);
                entity.Property(e => e.Balance_LCY).HasPrecision(18, 2);
                entity.Property(e => e.Balance_Due_LCY).HasPrecision(18, 2);
                entity.Property(e => e.Sales_LCY).HasPrecision(18, 2);
                entity.Property(e => e.Payments_LCY).HasPrecision(18, 2);

                // Kolom lain, misalnya:
                // entity.Property(e => e.Sales_LCY).HasPrecision(19, 4);
                // Sesuaikan dengan kebutuhan real data
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
