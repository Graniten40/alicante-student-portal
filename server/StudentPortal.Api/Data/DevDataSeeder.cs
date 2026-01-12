using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Auth;
using StudentPortal.Api.Models;

namespace StudentPortal.Api.Data;

public static class DevDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();

        // 0) Roles
        var roleNames = new[] { "Admin", "Student" };
        foreach (var r in roleNames)
        {
            if (!await roles.RoleExistsAsync(r))
                await roles.CreateAsync(new IdentityRole(r));
        }

        // 1) Product (paket)
        const string slug = "starter-pack";

        var product = await db.Products.SingleOrDefaultAsync(p => p.Slug == slug);
        if (product is null)
        {
            product = new Product
            {
                Slug = slug,
                TitleEn = "Starter Pack",
                TitleZh = "入门包",
                DescriptionEn = "Welcome to the portal. This is your first package.",
                DescriptionZh = "欢迎来到门户。这是你的第一个套餐。",
                PriceCents = 0,
                IsFree = true,
                IsActive = true
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();
        }

        // 2) PackContent (innehåll för produkten) — skapa bara om det saknas
        var hasContent = await db.PackContents.AnyAsync(pc => pc.ProductId == product.Id);
        if (!hasContent)
        {
            var json = """
            {
              "blocks": [
                { "type": "hero", "title": "Welcome!", "text": "Your portal is working." },
                { "type": "link", "label": "Go to docs", "href": "https://example.com" }
              ]
            }
            """;

            db.PackContents.Add(new PackContent
            {
                ProductId = product.Id,
                Version = 1,
                JsonContent = json,
                PublishedAt = DateTimeOffset.UtcNow
            });

            await db.SaveChangesAsync();
        }

        // 3) Entitlement för en test-user
        const string devEmail = "johan@test.se";

        var user = await users.Users.SingleOrDefaultAsync(u => u.Email == devEmail);

        if (user is null)
        {
            user = new AppUser
            {
                UserName = devEmail,
                Email = devEmail,
                DisplayName = "Johan Dev",
                PreferredLanguage = "en"
            };

            var created = await users.CreateAsync(user, "Password123!");
            if (!created.Succeeded)
                return;
        }

        // ✅ Make dev user Admin
        if (!await users.IsInRoleAsync(user, "Admin"))
            await users.AddToRoleAsync(user, "Admin");

        var alreadyEntitled = await db.Entitlements.AnyAsync(e =>
            e.UserId == user.Id && e.ProductId == product.Id);

        if (!alreadyEntitled)
        {
            db.Entitlements.Add(new Entitlement
            {
                UserId = user.Id,
                ProductId = product.Id,
                Source = "Free",
                ValidUntil = null,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }
}
