using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Data;
using StudentPortal.Api.Dtos;

namespace StudentPortal.Api.Services;

public sealed class PortalService : IPortalService
{
    private readonly AppDbContext _db;

    public PortalService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MyPackageDto>> GetMyPackagesAsync(string userId, string lang, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        lang = (lang == "zh") ? "zh" : "en";

        var items = await _db.Entitlements
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .Join(_db.Products.AsNoTracking(),
                e => e.ProductId,
                p => p.Id,
                (e, p) => new { e, p })
            .Select(x => new MyPackageDto(
                x.p.Id,
                x.p.Slug,
                lang == "zh" ? x.p.TitleZh : x.p.TitleEn,
                x.e.ValidUntil,
                x.e.ValidUntil == null || x.e.ValidUntil > now
            ))
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Title)
            .ToListAsync(ct);

        return items;
    }

    public async Task<PortalAccessResult> GetPackageIfEntitledAsync(
        string userId,
        string packageSlug,
        string lang,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        lang = (lang == "zh") ? "zh" : "en";

        var pkg = await _db.Products
            .AsNoTracking()
            .Where(p => p.Slug == packageSlug)
            .Select(p => new
            {
                p.Id,
                p.Slug,
                Title = lang == "zh" ? p.TitleZh : p.TitleEn,
                Description = lang == "zh" ? p.DescriptionZh : p.DescriptionEn
            })
            .SingleOrDefaultAsync(ct);

        if (pkg is null)
            return new PortalAccessResult(PortalAccessStatus.NotFound, null);

        var entitled = await _db.Entitlements
            .AsNoTracking()
            .AnyAsync(e =>
                e.UserId == userId &&
                e.ProductId == pkg.Id &&
                (e.ValidUntil == null || e.ValidUntil > now),
                ct);

        if (!entitled)
            return new PortalAccessResult(PortalAccessStatus.Forbidden, null);

        // OBS: singular "content" (senaste publicerade)
        var content = await _db.PackContents
            .AsNoTracking()
            .Where(pc => pc.ProductId == pkg.Id)
            .OrderByDescending(pc => pc.PublishedAt)
            .Select(pc => new PortalPackageContentDto(
                pc.Version,
                pc.JsonContent,
                pc.PublishedAt
            ))
            .FirstOrDefaultAsync(ct);

        // ✅ RÄTT: skapa PortalPackageDto (inte PortalPackageContentDto)
        var dto = new PortalPackageDto(
            pkg.Id,
            pkg.Slug,
            pkg.Title,
            pkg.Description,
            content
        );

        return new PortalAccessResult(PortalAccessStatus.Ok, dto);
    }
}
