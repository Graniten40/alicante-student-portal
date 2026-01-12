using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Auth;
using StudentPortal.Api.Models;
using StudentPortal.Api.Domain;
using StudentPortal.Api.Domain.Housing;

namespace StudentPortal.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    // New modules
    public DbSet<StudentHousing> StudentHousing => Set<StudentHousing>();
    public DbSet<StudentContactLog> StudentContactLogs => Set<StudentContactLog>();
    public DbSet<StudentTimelineEvent> StudentTimelineEvents => Set<StudentTimelineEvent>();

    // Existing
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Entitlement> Entitlements => Set<Entitlement>();
    public DbSet<PackContent> PackContents => Set<PackContent>();
    public DbSet<ProgressItem> Progress => Set<ProgressItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // ---------------------------
        // Housing (1:1 per student)
        // ---------------------------
        b.Entity<StudentHousing>()
            .HasIndex(x => x.StudentId)
            .IsUnique();

        b.Entity<StudentHousing>()
            .HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------------------------
        // Contact log (many per student)
        // + CreatedByUser relation
        // ---------------------------
        b.Entity<StudentContactLog>()
            .HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<StudentContactLog>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------
        // Timeline events (many per student)
        // + CreatedByUser relation
        // ---------------------------
        b.Entity<StudentTimelineEvent>()
            .HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<StudentTimelineEvent>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------
        // Existing models
        // ---------------------------
        b.Entity<Product>()
            .HasIndex(x => x.Slug)
            .IsUnique();

        b.Entity<Entitlement>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

        b.Entity<PackContent>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

        b.Entity<ProgressItem>()
            .HasIndex(x => new { x.UserId, x.ProductId, x.StepKey, x.TaskKey })
            .IsUnique();
    }
}
