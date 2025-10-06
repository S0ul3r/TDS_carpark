using CarParkApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarParkApi.Data;

public class CarParkDbContext : DbContext
{
    // how many spaces we want in car park
    private const int TotalSpaces = 20;
    
    public CarParkDbContext(DbContextOptions<CarParkDbContext> options)
        : base(options)
    {
    }

    public DbSet<ParkingSpace> ParkingSpaces { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ParkingSpace>(entity =>
        {
            entity.ToTable("parking_spaces");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.VehicleReg)
                .HasColumnName("vehicle_reg")
                .HasMaxLength(20);
            
            entity.Property(e => e.VehicleType)
                .HasColumnName("vehicle_type")
                .HasConversion<string>();
            
            entity.Property(e => e.SpaceNumber)
                .HasColumnName("space_number")
                .IsRequired();
            
            entity.Property(e => e.TimeIn)
                .HasColumnName("time_in");
            
            entity.Property(e => e.TimeOut)
                .HasColumnName("time_out");
            
            entity.Property(e => e.IsOccupied)
                .HasColumnName("is_occupied")
                .IsRequired()
                .HasDefaultValue(false);

            entity.HasIndex(e => e.VehicleReg)
                .IsUnique()
                .HasFilter("vehicle_reg IS NOT NULL");

            entity.HasIndex(e => e.SpaceNumber)
                .IsUnique();
        });

        // seed empty spaces (20 spaces)
        var spaces = Enumerable.Range(1, TotalSpaces)
            .Select(i => new ParkingSpace
            {
                Id = i,
                SpaceNumber = i,
                IsOccupied = false
            });
        
        modelBuilder.Entity<ParkingSpace>().HasData(spaces);
    }
}

