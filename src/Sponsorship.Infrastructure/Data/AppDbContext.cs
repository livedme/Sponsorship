using Microsoft.EntityFrameworkCore;
using Sponsorship.Application.Interfaces;
using Sponsorship.Domain.Entities;

namespace Sponsorship.Infrastructure.Data;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<SponsorshipRequest> SponsorshipRequests => Set<SponsorshipRequest>();
    public DbSet<WorkflowHistory> WorkflowHistories => Set<WorkflowHistory>();
    public DbSet<SponsorshipType> SponsorshipTypes => Set<SponsorshipType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        modelBuilder.Entity<SponsorshipRequest>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.RequestedAmount).HasColumnType("decimal(18,2)");
            e.Property(r => r.Status).HasConversion<string>();

            e.HasOne(r => r.Requestor)
                .WithMany(u => u.Requests)
                .HasForeignKey(r => r.RequestorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(r => r.SponsorshipType)
                .WithMany(t => t.Requests)
                .HasForeignKey(r => r.SponsorshipTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkflowHistory>(e =>
        {
            e.HasKey(h => h.Id);
            e.Property(h => h.PreviousStatus).HasConversion<string>();
            e.Property(h => h.NewStatus).HasConversion<string>();

            e.HasOne(h => h.Request)
                .WithMany(r => r.WorkflowHistory)
                .HasForeignKey(h => h.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(h => h.ActionBy)
                .WithMany(u => u.WorkflowActions)
                .HasForeignKey(h => h.ActionById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
