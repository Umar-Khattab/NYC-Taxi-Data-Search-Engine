using Microsoft.EntityFrameworkCore;
using NycTaxiSearch.Models;
using System.Collections.Generic;

namespace NycTaxiSearch.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<TaxiTrip> TaxiTrips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes for better query performance
            modelBuilder.Entity<TaxiTrip>()
                .HasIndex(t => t.PickupDateTime)
                .HasDatabaseName("IX_TaxiTrips_PickupDateTime");

            modelBuilder.Entity<TaxiTrip>()
                .HasIndex(t => t.PassengerCount)
                .HasDatabaseName("IX_TaxiTrips_PassengerCount");

            modelBuilder.Entity<TaxiTrip>()
                .HasIndex(t => t.TripDistance)
                .HasDatabaseName("IX_TaxiTrips_TripDistance");

            modelBuilder.Entity<TaxiTrip>()
                .HasIndex(t => t.FareAmount)
                .HasDatabaseName("IX_TaxiTrips_FareAmount");

            modelBuilder.Entity<TaxiTrip>()
                .HasIndex(t => t.PaymentType)
                .HasDatabaseName("IX_TaxiTrips_PaymentType");
        }
    }
}