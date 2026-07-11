using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GigBartending.Api.Models;

namespace GigBartending.Api.Data;

public class GigBartendingDbContext : IdentityDbContext<ApplicationUser>
{
    public GigBartendingDbContext(DbContextOptions<GigBartendingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Shift> Shifts { get; set; }
    public DbSet<ShiftRequest> ShiftRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser entity
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(u => u.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Configure Shift entity
        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).IsRequired().HasMaxLength(255);
            entity.Property(s => s.Description).HasMaxLength(1000);
            entity.Property(s => s.Location).IsRequired().HasMaxLength(255);
            entity.Property(s => s.Status).IsRequired().HasMaxLength(50);
            entity.Property(s => s.HourlyRate).HasColumnType("decimal(18,2)");
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(s => s.Venue)
                .WithMany(u => u.PostedShifts)
                .HasForeignKey(s => s.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.AcceptedByUser)
                .WithMany(u => u.AcceptedShifts)
                .HasForeignKey(s => s.AcceptedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure ShiftRequest entity
        modelBuilder.Entity<ShiftRequest>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Status).IsRequired().HasMaxLength(50);
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(r => new { r.ShiftId, r.BartenderId }).IsUnique();

            entity.HasOne(r => r.Shift)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Bartender)
                .WithMany(u => u.ShiftRequests)
                .HasForeignKey(r => r.BartenderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
