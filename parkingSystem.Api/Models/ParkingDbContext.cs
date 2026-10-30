using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace parkingSystem.Api.Models;

public partial class ParkingDbContext : DbContext
{
    public ParkingDbContext()
    {
    }

    public ParkingDbContext(DbContextOptions<ParkingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<ParkingSpot> ParkingSpots { get; set; }

    public virtual DbSet<ParkingTicket> ParkingTickets { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PricingRate> PricingRates { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VwActiveParkingSession> VwActiveParkingSessions { get; set; }

    public virtual DbSet<VwDailyCollection> VwDailyCollections { get; set; }

    public virtual DbSet<VwOccupancySummary> VwOccupancySummaries { get; set; }

    public virtual DbSet<VwPaymentHistory> VwPaymentHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=parking_system_db;user=root;password=Leoul09307#", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("PRIMARY");

            entity.ToTable("drivers");

            entity.HasIndex(e => e.LicenseNumber, "idx_driver_license");

            entity.HasIndex(e => e.FullName, "idx_driver_name");

            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(50)
                .HasColumnName("license_number");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<ParkingSpot>(entity =>
        {
            entity.HasKey(e => e.SpotId).HasName("PRIMARY");

            entity.ToTable("parking_spots");

            entity.HasIndex(e => e.CustomRateId, "fk_spot_rate");

            entity.HasIndex(e => new { e.IsOccupied, e.IsActive }, "idx_spot_status");

            entity.HasIndex(e => e.SpotType, "idx_spot_type");

            entity.HasIndex(e => e.SpotCode, "spot_code").IsUnique();

            entity.Property(e => e.SpotId).HasColumnName("spot_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomRateId).HasColumnName("custom_rate_id");
            entity.Property(e => e.FloorNumber)
                .HasDefaultValueSql("'1'")
                .HasColumnName("floor_number");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.IsOccupied).HasColumnName("is_occupied");
            entity.Property(e => e.SpotCode)
                .HasMaxLength(20)
                .HasColumnName("spot_code");
            entity.Property(e => e.SpotType)
                .HasDefaultValueSql("'STANDARD'")
                .HasColumnType("enum('STANDARD','COMPACT','EV_CHARGING','HANDICAPPED','VIP')")
                .HasColumnName("spot_type");

            entity.HasOne(d => d.CustomRate).WithMany(p => p.ParkingSpots)
                .HasForeignKey(d => d.CustomRateId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_spot_rate");
        });

        modelBuilder.Entity<ParkingTicket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PRIMARY");

            entity.ToTable("parking_tickets");

            entity.HasIndex(e => e.DriverId, "fk_ticket_driver");

            entity.HasIndex(e => e.EntryTime, "idx_ticket_entry");

            entity.HasIndex(e => e.LicensePlate, "idx_ticket_plate");

            entity.HasIndex(e => e.Status, "idx_ticket_status");

            entity.HasIndex(e => e.RateId, "rate_id");

            entity.HasIndex(e => e.SpotId, "spot_id");

            entity.HasIndex(e => e.TicketNumber, "ticket_number").IsUnique();

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.EntryTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("entry_time");
            entity.Property(e => e.ExitTime)
                .HasColumnType("datetime")
                .HasColumnName("exit_time");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.RateId).HasColumnName("rate_id");
            entity.Property(e => e.SpotId).HasColumnName("spot_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'ACTIVE'")
                .HasColumnType("enum('ACTIVE','PAID','COMPLETED','CANCELLED')")
                .HasColumnName("status");
            entity.Property(e => e.TicketNumber)
                .HasMaxLength(50)
                .HasColumnName("ticket_number");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.TotalDurationMinutes).HasColumnName("total_duration_minutes");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Driver).WithMany(p => p.ParkingTickets)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ticket_driver");

            entity.HasOne(d => d.Rate).WithMany(p => p.ParkingTickets)
                .HasForeignKey(d => d.RateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("parking_tickets_ibfk_2");

            entity.HasOne(d => d.Spot).WithMany(p => p.ParkingTickets)
                .HasForeignKey(d => d.SpotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("parking_tickets_ibfk_1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.TicketId, "idx_payment_ticket");

            entity.HasIndex(e => e.PaymentTime, "idx_payment_time");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CashierName)
                .HasMaxLength(100)
                .HasColumnName("cashier_name");
            entity.Property(e => e.PaymentMethod)
                .HasColumnType("enum('CASH','CREDIT_CARD','DEBIT_CARD','MOBILE_PAY')")
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasDefaultValueSql("'SUCCESS'")
                .HasColumnType("enum('SUCCESS','FAILED','REFUNDED')")
                .HasColumnName("payment_status");
            entity.Property(e => e.PaymentTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("payment_time");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(100)
                .HasColumnName("reference_number");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("payments_ibfk_1");
        });

        modelBuilder.Entity<PricingRate>(entity =>
        {
            entity.HasKey(e => e.RateId).HasName("PRIMARY");

            entity.ToTable("pricing_rates");

            entity.HasIndex(e => new { e.VehicleType, e.IsActive }, "idx_vehicle_rate");

            entity.Property(e => e.RateId).HasColumnName("rate_id");
            entity.Property(e => e.AdditionalHourlyRate)
                .HasPrecision(8, 2)
                .HasDefaultValueSql("'3.00'")
                .HasColumnName("additional_hourly_rate");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.FirstHourRate)
                .HasPrecision(8, 2)
                .HasDefaultValueSql("'5.00'")
                .HasColumnName("first_hour_rate");
            entity.Property(e => e.GracePeriodMins)
                .HasDefaultValueSql("'15'")
                .HasColumnName("grace_period_mins");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.MaxDailyRate)
                .HasPrecision(8, 2)
                .HasDefaultValueSql("'35.00'")
                .HasColumnName("max_daily_rate");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.VehicleType)
                .HasDefaultValueSql("'CAR'")
                .HasColumnType("enum('MOTORCYCLE','CAR','SUV','TRUCK')")
                .HasColumnName("vehicle_type");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PRIMARY");

            entity.ToTable("vehicles");

            entity.HasIndex(e => e.LicensePlate, "idx_plate").IsUnique();

            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.Color)
                .HasMaxLength(30)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.MakeModel)
                .HasMaxLength(50)
                .HasColumnName("make_model");
            entity.Property(e => e.VehicleType)
                .HasDefaultValueSql("'CAR'")
                .HasColumnType("enum('MOTORCYCLE','CAR','SUV','TRUCK')")
                .HasColumnName("vehicle_type");
        });

        modelBuilder.Entity<VwActiveParkingSession>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_active_parking_sessions");

            entity.Property(e => e.CurrentAccumulatedFee)
                .HasPrecision(10, 2)
                .HasColumnName("current_accumulated_fee");
            entity.Property(e => e.DriverName)
                .HasMaxLength(100)
                .HasColumnName("driver_name");
            entity.Property(e => e.DriverPhone)
                .HasMaxLength(20)
                .HasColumnName("driver_phone");
            entity.Property(e => e.ElapsedMinutes).HasColumnName("elapsed_minutes");
            entity.Property(e => e.EntryTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("entry_time");
            entity.Property(e => e.FloorNumber)
                .HasDefaultValueSql("'1'")
                .HasColumnName("floor_number");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.PricingPlan)
                .HasMaxLength(50)
                .HasColumnName("pricing_plan");
            entity.Property(e => e.SpotCode)
                .HasMaxLength(20)
                .HasColumnName("spot_code");
            entity.Property(e => e.SpotType)
                .HasDefaultValueSql("'STANDARD'")
                .HasColumnType("enum('STANDARD','COMPACT','EV_CHARGING','HANDICAPPED','VIP')")
                .HasColumnName("spot_type");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.TicketNumber)
                .HasMaxLength(50)
                .HasColumnName("ticket_number");
        });

        modelBuilder.Entity<VwDailyCollection>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_daily_collections");

            entity.Property(e => e.CashierName)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("cashier_name");
            entity.Property(e => e.CollectionDate).HasColumnName("collection_date");
            entity.Property(e => e.PaymentMethod)
                .HasColumnType("enum('CASH','CREDIT_CARD','DEBIT_CARD','MOBILE_PAY')")
                .HasColumnName("payment_method");
            entity.Property(e => e.TotalCollected)
                .HasPrecision(32, 2)
                .HasColumnName("total_collected");
            entity.Property(e => e.TransactionCount).HasColumnName("transaction_count");
        });

        modelBuilder.Entity<VwOccupancySummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_occupancy_summary");

            entity.Property(e => e.AvailableSpots)
                .HasPrecision(23)
                .HasColumnName("available_spots");
            entity.Property(e => e.OccupancyPercentage)
                .HasPrecision(28, 1)
                .HasColumnName("occupancy_percentage");
            entity.Property(e => e.OccupiedSpots)
                .HasPrecision(23)
                .HasColumnName("occupied_spots");
            entity.Property(e => e.TotalSpots).HasColumnName("total_spots");
        });

        modelBuilder.Entity<VwPaymentHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_payment_history");

            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CashierName)
                .HasMaxLength(100)
                .HasColumnName("cashier_name");
            entity.Property(e => e.DriverName)
                .HasMaxLength(100)
                .HasColumnName("driver_name");
            entity.Property(e => e.DriverPhone)
                .HasMaxLength(20)
                .HasColumnName("driver_phone");
            entity.Property(e => e.EntryTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("entry_time");
            entity.Property(e => e.ExitTime)
                .HasColumnType("datetime")
                .HasColumnName("exit_time");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.PaymentMethod)
                .HasColumnType("enum('CASH','CREDIT_CARD','DEBIT_CARD','MOBILE_PAY')")
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("payment_time");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(100)
                .HasColumnName("reference_number");
            entity.Property(e => e.SpotCode)
                .HasMaxLength(20)
                .HasColumnName("spot_code");
            entity.Property(e => e.TicketNumber)
                .HasMaxLength(50)
                .HasColumnName("ticket_number");
            entity.Property(e => e.TotalDurationMinutes).HasColumnName("total_duration_minutes");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
