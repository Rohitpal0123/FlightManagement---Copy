using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FlightManagement.Models;

public partial class FlightManagementContext : DbContext
{
    public FlightManagementContext()
    {
    }

    public FlightManagementContext(DbContextOptions<FlightManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LIN-5CG4401ZNQ\\SQLEXPRESS;Database=FlightManagement;User ID=sa;Password=rohit@321@S;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
{
    entity.HasKey(e => e.BookingId).HasName("PK__Bookings__C6D03BCDE2A43D58");

    entity.Property(e => e.BookingId).HasColumnName("bookingId");
    entity.Property(e => e.TotalPrice)
        .HasPrecision(18, 2);
    entity.Property(e => e.BookingDate)
        .HasDefaultValueSql("(getdate())")
        .HasColumnType("datetime")
        .HasColumnName("bookingDate");
    entity.Property(e => e.FlightId).HasColumnName("flightId");
    entity.Property(e => e.FlightName)
        .HasMaxLength(5)
        .HasColumnName("flightName");
    entity.Property(e => e.IsCancelled)
        .HasDefaultValue(false)
        .HasColumnName("isCancelled");
    entity.Property(e => e.IsCheckedOut)  // Use the correct casing here
        .HasDefaultValue(false)
        .HasColumnName("isCheckedOut"); // Make sure the column name matches the property name
    entity.Property(e => e.IsDeleted)
        .HasDefaultValue(false)
        .HasColumnName("isDeleted");
    entity.Property(e => e.NumOfPassengers).HasColumnName("numOfPassengers");
    entity.Property(e => e.SeatNumber)
        .HasMaxLength(10)
        .HasColumnName("seatNumber");
    entity.Property(e => e.TotalPrice)
        .HasColumnName("totalPrice");
    entity.Property(e => e.UserId).HasColumnName("userId");

    entity.HasOne(d => d.Flight).WithMany(p => p.BookingFlights)
        .HasForeignKey(d => d.FlightId)
        .HasConstraintName("FK__Bookings__flight__4AB81AF0");

    entity.HasOne(d => d.FlightNameNavigation).WithMany(p => p.BookingFlightNameNavigations)
        .HasPrincipalKey(p => p.FlightName)
        .HasForeignKey(d => d.FlightName)
        .HasConstraintName("FK_Bookings_FlightName");

    entity.HasOne(d => d.User).WithMany(p => p.Bookings)
        .HasForeignKey(d => d.UserId)
        .HasConstraintName("FK__Bookings__userId__4BAC3F29");
});


        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("PK__Flights__0E018642ED4E68EA");

            entity.HasIndex(e => e.FlightName, "UQ_Flights_FlightName").IsUnique();

            entity.Property(e => e.FlightId).HasColumnName("flightId");
            entity.Property(e => e.Arrival)
                .HasColumnType("datetime")
                .HasColumnName("arrival");
            entity.Property(e => e.Departure)
                .HasColumnType("datetime")
                .HasColumnName("departure");
            entity.Property(e => e.Destination)
                .HasMaxLength(100)
                .HasColumnName("destination");
            entity.Property(e => e.FlightName)
                .HasMaxLength(5)
                .HasColumnName("flightName");
            entity.Property(e => e.IsCancelled)
                .HasDefaultValue(false)
                .HasColumnName("isCancelled");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.SeatsAvailable).HasColumnName("seatsAvailable");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .HasColumnName("source");
            entity.Property(e => e.TotalSeats).HasColumnName("totalSeats");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__CD98462A7B971181");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__CB9A1CFF7C0443A7");

            entity.HasIndex(e => e.Contact, "UQ__Users__870C3C8B00EA86B4").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164FEAFD4B0").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Contact)
                .HasMaxLength(15)
                .HasColumnName("contact");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleId");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__roleId__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
