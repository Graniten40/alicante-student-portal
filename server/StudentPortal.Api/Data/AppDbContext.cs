using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Auth;
using StudentPortal.Api.Models;

namespace StudentPortal.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Entitlement> Entitlements => Set<Entitlement>();
    public DbSet<PackContent> PackContents => Set<PackContent>();
    public DbSet<ProgressItem> Progress => Set<ProgressItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

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
